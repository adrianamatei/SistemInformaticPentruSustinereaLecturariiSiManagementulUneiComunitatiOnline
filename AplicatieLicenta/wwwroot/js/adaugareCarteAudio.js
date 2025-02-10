
    const setupDropzoneWithPreview = (dropzoneId, inputId, allowedTypePrefix, previewId) =>
    {
        const dropzone = document.getElementById(dropzoneId);
    const input = document.getElementById(inputId);
    const preview = document.getElementById(previewId);

        dropzone.addEventListener('dragover', (e) =>
    {
        e.preventDefault();
    dropzone.classList.add('dragover');
        });

        dropzone.addEventListener('dragleave', () =>
    {
        dropzone.classList.remove('dragover');
        });

        dropzone.addEventListener('drop', (e) =>
    {
        e.preventDefault();
    dropzone.classList.remove('dragover');
    const file = e.dataTransfer.files[0];

    if (file && file.type.startsWith(allowedTypePrefix))
    {
        input.files = e.dataTransfer.files;
    dropzone.querySelector('p').textContent = file.name;

    // Previzualizarea imaginii
    if (allowedTypePrefix === 'image/')
    {
                    const reader = new FileReader();
                    reader.onload = (e) => {
        preview.src = e.target.result;
    preview.style.display = 'block';
                    };
    reader.readAsDataURL(file);
                }
            }
    else
    {
        alert(`Fișier invalid. Selectați un fișier de tip ${allowedTypePrefix}.`);
            }
        });

        dropzone.addEventListener('click', () => input.click());

        input.addEventListener('change', () => {
            const file = input.files[0];

    if (file && file.type.startsWith(allowedTypePrefix))
    {
        dropzone.querySelector('p').textContent = file.name;

    // Previzualizarea imaginii
    if (allowedTypePrefix === 'image/')
    {
                    const reader = new FileReader();
                    reader.onload = (e) =>
    {
        preview.src = e.target.result;
    preview.style.display = 'block';
                    };
    reader.readAsDataURL(file);
                }
            }
    else
    {
        alert(`Fișier invalid. Selectați un fișier de tip ${allowedTypePrefix}.`);
            }
        });
    };

   
    setupDropzoneWithPreview('dropzone-coperta', 'Imagine_Coperta', 'image/', 'preview-coperta');
    const setupDropzone = (dropzoneId, inputId, allowedType) =>
    {
        const dropzone = document.getElementById(dropzoneId);
    const input = document.getElementById(inputId);

        dropzone.addEventListener('dragover', (e) =>
    {
        e.preventDefault();
    dropzone.classList.add('dragover');
        });

        dropzone.addEventListener('dragleave', () =>
    {
        dropzone.classList.remove('dragover');
        });

        dropzone.addEventListener('drop', (e) =>
    {
        e.preventDefault();
    dropzone.classList.remove('dragover');
    const file = e.dataTransfer.files[0];

    if (file && (file.type === allowedType || file.type === 'audio/mpeg'))
    {
        input.files = e.dataTransfer.files;
    dropzone.querySelector('p').textContent = file.name;
            }
    else
    {
        alert(`Fișier invalid. Selectați un fișier de tip ${allowedType}.`);
            }
        });

        dropzone.addEventListener('click', () => input.click());

        input.addEventListener('change', () => {
            const file = input.files[0];

    if (file && (file.type === allowedType || file.type === 'audio/mpeg'))
    {
        dropzone.querySelector('p').textContent = file.name;
            }
    else
    {
        alert(`Fișier invalid. Selectați un fișier de tip ${allowedType}.`);
            }
        });
    };
    setupDropzone('dropzone-audio', 'url_fisier', 'audio/mpeg');
