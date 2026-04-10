function confirmDelete(btn, carTitle) {
    Swal.fire({
        title: 'Delete Review?',
        text: 'Remove your review for "' + carTitle + '"?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
