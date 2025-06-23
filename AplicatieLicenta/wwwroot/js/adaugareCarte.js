document.addEventListener("DOMContentLoaded", function ()
{
    const setupDropzoneWithPreview = (dropzoneId, inputId, allowedTypePrefix, previewId) =>
    {
        const dropzone = document.getElementById(dropzoneId);
        const input = document.getElementById(inputId);
        const preview = document.getElementById(previewId);

        if (!dropzone || !input || !preview)
        {
            console.error('Eroare: Unul dintre elemente lipsește: ${dropzoneId}, ${inputId}, ${previewId}');
            return;
        }

        let fileSelectedViaDrop = false;

        dropzone.addEventListener("dragover", (e) =>
        {
            e.preventDefault();
            dropzone.classList.add("dragover");
        });

        dropzone.addEventListener("dragleave", () =>
        {
            dropzone.classList.remove("dragover");
        });

        dropzone.addEventListener("drop", (e) =>
        {
            e.preventDefault();
            dropzone.classList.remove("dragover");

            const file = e.dataTransfer.files[0];

            if (file && file.type.startsWith(allowedTypePrefix))
            {
                fileSelectedViaDrop = true;
                input.value = "";

                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(file);
                input.files = dataTransfer.files;

                dropzone.querySelector("p").textContent = file.name;

                const reader = new FileReader();
                reader.onload = (event) => {
                    preview.src = event.target.result;
                    preview.style.display = "block";
                };
                reader.readAsDataURL(file);
            }
            else
            {
                alert("Fișier invalid. Selectați o imagine.");
            }
        });

        dropzone.addEventListener("click", () =>
        {
            if (!fileSelectedViaDrop) {
                input.click();
            }
        });

        input.addEventListener("change", () =>
        {
            const file = input.files[0];

            if (file && file.type.startsWith(allowedTypePrefix))
            {
                dropzone.querySelector("p").textContent = file.name;
                fileSelectedViaDrop = false;

                const reader = new FileReader();
                reader.onload = (event) => {
                    preview.src = event.target.result;
                    preview.style.display = "block";
                };
                reader.readAsDataURL(file);
            }
            else
            {
                alert("Fișier invalid. Selectați o imagine.");
            }
        });
    };

    setupDropzoneWithPreview("dropzone-coperta", "ImagineCoperta", "image/", "preview-coperta");

    const setupDropzone = (dropzoneId, inputId, allowedTypePrefix) =>
    {
        const dropzone = document.getElementById(dropzoneId);
        const input = document.getElementById(inputId);

        if (!dropzone || !input) {
            console.error('Eroare: Unul dintre elemente lipsește: ${dropzoneId}, ${inputId}');
            return;
        }

        dropzone.addEventListener("dragover", (e) =>
        {
            e.preventDefault();
            dropzone.classList.add("dragover");
        });

        dropzone.addEventListener("dragleave", () =>
        {
            dropzone.classList.remove("dragover");
        });

        dropzone.addEventListener("drop", (e) =>
        {
            e.preventDefault();
            dropzone.classList.remove("dragover");

            const file = e.dataTransfer.files[0];

            if (file && file.type.startsWith(allowedTypePrefix))
            {
                input.value = "";

                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(file);
                input.files = dataTransfer.files;

                dropzone.querySelector("p").textContent = file.name;
            }
            else
            {
                alert("Fisier invalid. Selectati un fisier PDF.");
            }
        });

        dropzone.addEventListener("click", () => input.click());

        input.addEventListener("change", () =>
        {
            const file = input.files[0];

            if (file && file.type.startsWith(allowedTypePrefix))
            {
                dropzone.querySelector("p").textContent = file.name;
            }
            else
            {
                alert("Fisier invalid. Selectati un fisier PDF.");
            }
        });
    };

    setupDropzone("dropzone-pdf", "UrlFisier", "application/pdf");
});
