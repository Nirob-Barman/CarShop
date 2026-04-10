function confirmDelete(btn, reviewer) {
    Swal.fire({
        title: 'Delete Review?',
        text: 'Remove review by "' + reviewer + '"?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
