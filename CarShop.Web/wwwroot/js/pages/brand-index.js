function confirmDeleteBrand(btn, name) {
    Swal.fire({
        title: 'Delete brand?',
        text: "Delete '" + name + "'?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete',
        cancelButtonText: 'Cancel'
    }).then(function (result) {
        if (result.isConfirmed) btn.closest('form').submit();
    });
}
