let scene, camera, renderer, cupModel, controls;
let decals = [];
let activeDecal = null;
let targetMesh = null;
let decalIdCounter = 0;
let isDragging = false;

window.init3DConstructor = (containerId, modelPath) => {
    const container = document.getElementById(containerId);
    if (!container) return;

    scene = new THREE.Scene();
    scene.background = new THREE.Color(0xf8f9fa);

    camera = new THREE.PerspectiveCamera(45, container.clientWidth / container.clientHeight, 0.1, 1000);
    camera.position.set(0, 1.5, 5);

    renderer = new THREE.WebGLRenderer({ antialias: true, preserveDrawingBuffer: true });
    renderer.setSize(container.clientWidth, container.clientHeight);
    renderer.setPixelRatio(window.devicePixelRatio);
    container.appendChild(renderer.domElement);

    controls = new THREE.OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true;

    scene.add(new THREE.AmbientLight(0xffffff, 0.9));
    const mainLight = new THREE.DirectionalLight(0xffffff, 1.0);
    mainLight.position.set(5, 10, 7);
    scene.add(mainLight);

    const loader = new THREE.GLTFLoader();
    loader.load(modelPath, (gltf) => {
        cupModel = gltf.scene;
        cupModel.traverse(node => {
            if (node.isMesh && (node.name.includes("Cylinder001_1") || node.name.toLowerCase().includes("print"))) {
                targetMesh = node;
            }
        });

        const box = new THREE.Box3().setFromObject(cupModel);
        const size = box.getSize(new THREE.Vector3());
        const maxDim = Math.max(size.x, size.y, size.z);
        const scaleFactor = 3.5 / (maxDim > 0 ? maxDim : 1);
        cupModel.scale.set(scaleFactor, scaleFactor, scaleFactor);
        const center = box.getCenter(new THREE.Vector3());
        cupModel.position.sub(center.multiplyScalar(scaleFactor));

        scene.add(cupModel);
    });

    let startX, startY;
    renderer.domElement.addEventListener('pointerdown', (e) => { startX = e.clientX; startY = e.clientY; });
    renderer.domElement.addEventListener('pointerup', (e) => {
        const diffX = Math.abs(e.clientX - startX);
        const diffY = Math.abs(e.clientY - startY);
        if (diffX < 5 && diffY < 5) handleManualClick(e, container);
    });

    animate();
};

window.updateModelImage = (imageUrl) => {
    return new Promise((resolve) => {
        const texLoader = new THREE.TextureLoader();
        texLoader.setCrossOrigin('anonymous');
        texLoader.load(imageUrl, (texture) => {
            texture.flipY = false;
            texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
            texture.repeat.y = -1;
            texture.offset.y = 1;

            const material = new THREE.MeshPhongMaterial({
                map: texture, transparent: true, depthTest: true,
                depthWrite: false, polygonOffset: true, polygonOffsetFactor: -10, shininess: 30
            });

            const id = "layer_" + (decalIdCounter++);
            const position = new THREE.Vector3(0, 0, 1.5);
            const orientation = new THREE.Euler(0, 0, 0);
            const size = new THREE.Vector3(1, 1, 1);

            const geometry = new THREE.DecalGeometry(targetMesh, position, orientation, size);
            const decalMesh = new THREE.Mesh(geometry, material);
            decalMesh.name = id;
            decalMesh.userData = { id, position, orientation, size, material };

            scene.add(decalMesh);
            decals.push(decalMesh);
            activeDecal = decalMesh;
            resolve(id);
        });
    });
};

function handleManualClick(event, container) {
    if (!activeDecal || !cupModel) return;
    const rect = renderer.domElement.getBoundingClientRect();
    const mouse = new THREE.Vector2();
    mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
    mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

    const raycaster = new THREE.Raycaster();
    raycaster.setFromCamera(mouse, camera);
    const intersects = raycaster.intersectObject(cupModel, true);

    if (intersects.length > 0) {
        const intersection = intersects.find(i => !decals.includes(i.object));
        if (intersection) {
            activeDecal.userData.position.copy(intersection.point);
            const n = intersection.face.normal.clone();
            n.transformDirection(intersection.object.matrixWorld);
            const dummy = new THREE.Object3D();
            dummy.position.copy(intersection.point);
            dummy.lookAt(new THREE.Vector3().copy(intersection.point).add(n));
            activeDecal.userData.orientation.copy(dummy.rotation);
            refreshDecal(activeDecal);
        }
    }
}

function refreshDecal(decal) {
    const data = decal.userData;
    scene.remove(decal);
    const geometry = new THREE.DecalGeometry(targetMesh, data.position, data.orientation, data.size);
    const newMesh = new THREE.Mesh(geometry, data.material);
    newMesh.name = data.id;
    newMesh.userData = data;
    const index = decals.findIndex(d => d.name === data.id);
    if (index !== -1) decals[index] = newMesh;
    activeDecal = newMesh;
    scene.add(newMesh);
}

window.updateModelScale = (scaleValue) => {
    if (!activeDecal) return;
    activeDecal.userData.size.set(scaleValue, scaleValue, scaleValue);
    refreshDecal(activeDecal);
};

window.removeSpecificDecal = (id) => {
    const index = decals.findIndex(d => d.name === id);
    if (index !== -1) {
        scene.remove(decals[index]);
        decals.splice(index, 1);
        activeDecal = decals.length > 0 ? decals[decals.length - 1] : null;
    }
};

window.getConstructorState = () => {
    return JSON.stringify(decals.map(d => ({
        pos: d.userData.position,
        rot: { x: d.userData.orientation.x, y: d.userData.orientation.y, z: d.userData.orientation.z },
        size: d.userData.size.x,
        img: d.userData.material.map.image.src
    })));
};

function animate() {
    requestAnimationFrame(animate);
    if (controls) controls.update();
    renderer.render(scene, camera);
}

// Функція для відтворення дизайну клієнта в адмінці
window.loadClientDesign = async (jsonState) => {
    if (!jsonState) return;

    // Чекаємо, поки модель повністю завантажиться, якщо targetMesh ще null
    if (!targetMesh) {
        console.log("Очікування завантаження моделі...");
        await new Promise(resolve => {
            const check = setInterval(() => {
                if (targetMesh) {
                    clearInterval(check);
                    resolve();
                }
            }, 100);
        });
    }

    const layers = JSON.parse(jsonState);

    for (const data of layers) {
        await new Promise((resolve) => {
            const texLoader = new THREE.TextureLoader();
            texLoader.load(data.img, (texture) => {
                texture.flipY = false;
                texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
                texture.repeat.y = -1;
                texture.offset.y = 1;

                const material = new THREE.MeshPhongMaterial({
                    map: texture,
                    transparent: true,
                    depthTest: true,
                    depthWrite: false,
                    polygonOffset: true,
                    polygonOffsetFactor: -10,
                    shininess: 30
                });

                // Використовуємо збережені координати
                const position = new THREE.Vector3(data.pos.x, data.pos.y, data.pos.z);
                const orientation = new THREE.Euler(data.rot.x, data.rot.y, data.rot.z);
                const size = new THREE.Vector3(data.size, data.size, data.size);

                const geometry = new THREE.DecalGeometry(targetMesh, position, orientation, size);
                const decalMesh = new THREE.Mesh(geometry, material);

                scene.add(decalMesh);
                decals.push(decalMesh);
                resolve();
            });
        });
    }
};