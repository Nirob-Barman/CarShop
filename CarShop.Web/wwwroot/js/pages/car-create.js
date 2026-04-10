(function () {
    var allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    var maxSize      = 2 * 1024 * 1024;
    var imageInput   = document.getElementById('imageInput');
    if (!imageInput) return;

    imageInput.addEventListener('change', function () {
        var file       = this.files[0];
        var errorEl    = document.getElementById('imageError');
        var previewBox = document.getElementById('previewBox');
        var previewImg = document.getElementById('previewImg');

        errorEl.textContent = '';
        previewBox.classList.add('d-none');
        if (!file) return;

        if (!allowedTypes.includes(file.type)) {
            errorEl.textContent = 'Please upload a valid image file (JPG, PNG, GIF, or WebP).';
            this.value = '';
            return;
        }
        if (file.size > maxSize) {
            errorEl.textContent = 'Image exceeds the 2 MB limit.';
            this.value = '';
            return;
        }

        var reader = new FileReader();
        reader.onload = function (e) {
            previewImg.src = e.target.result;
            previewBox.classList.remove('d-none');
        };
        reader.readAsDataURL(file);
    });

    document.getElementById('createCarForm').addEventListener('submit', function (e) {
        var file    = imageInput.files[0];
        var errorEl = document.getElementById('imageError');
        errorEl.textContent = '';

        if (!file) {
            errorEl.textContent = 'Please select an image file.';
            e.preventDefault();
            return;
        }
        if (!allowedTypes.includes(file.type)) {
            errorEl.textContent = 'Please upload a valid image file (JPG, PNG, GIF, or WebP).';
            e.preventDefault();
            return;
        }
        if (file.size > maxSize) {
            errorEl.textContent = 'Image exceeds the 2 MB limit.';
            e.preventDefault();
        }
    });
})();
