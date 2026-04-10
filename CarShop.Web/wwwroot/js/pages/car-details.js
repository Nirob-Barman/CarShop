function makeStarPicker(pickerId, hiddenInputId, starClass) {
    var picker = document.getElementById(pickerId);
    var hidden = document.getElementById(hiddenInputId);
    if (!picker || !hidden) return;
    var stars = Array.from(picker.querySelectorAll('.' + starClass));
    var selected = 0;
    function paint(upTo) { stars.forEach(function (s, i) { s.style.color = i < upTo ? '#ffc107' : '#dee2e6'; }); }
    stars.forEach(function (star, idx) {
        star.addEventListener('mouseover', function () { paint(idx + 1); });
        star.addEventListener('click', function () { selected = idx + 1; hidden.value = selected; paint(selected); });
    });
    picker.addEventListener('mouseleave', function () { paint(selected); });
    picker._setVal = function (val) { selected = val; hidden.value = val; paint(val); };
}

function openEditModal(commentId, rating, content) {
    document.getElementById('editCommentId').value = commentId;
    document.getElementById('editContent').value   = content;
    var picker = document.getElementById('editStarPicker');
    if (picker && picker._setVal) picker._setVal(rating);
    new bootstrap.Modal(document.getElementById('editReviewModal')).show();
}

function openDeleteModal(commentId, carId) {
    document.getElementById('deleteCommentId').value = commentId;
    document.getElementById('deleteCarId').value     = carId;
    new bootstrap.Modal(document.getElementById('deleteReviewModal')).show();
}

function confirmDeleteCar(title) {
    Swal.fire({
        title: 'Delete car?',
        text: "Delete '" + title + "'? This cannot be undone.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete'
    }).then(function (result) {
        if (result.isConfirmed) document.getElementById('deleteCarForm').submit();
    });
}

function initShareLinks(carTitle) {
    var url   = encodeURIComponent(window.location.href);
    var title = encodeURIComponent(carTitle);
    var wa = document.getElementById('shareWhatsApp');
    var fb = document.getElementById('shareFacebook');
    if (wa) wa.href = 'https://wa.me/?text=' + title + '%20' + url;
    if (fb) fb.href = 'https://www.facebook.com/sharer/sharer.php?u=' + url;
    var copyBtn = document.getElementById('copyLink');
    if (copyBtn) {
        copyBtn.addEventListener('click', function () {
            navigator.clipboard.writeText(window.location.href).then(function () {
                document.getElementById('copyLinkText').textContent = 'Copied!';
                setTimeout(function () { document.getElementById('copyLinkText').textContent = 'Copy Link'; }, 2000);
            });
        });
    }
}

// Auto-init star pickers on load
makeStarPicker('starPicker',     'ratingValue',     'sp-star');
makeStarPicker('editStarPicker', 'editRatingValue', 'ep-star');
