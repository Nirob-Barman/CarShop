function confirmDeleteCar(btn, title) {
    Swal.fire({
        title: 'Delete car?', text: "Delete '" + title + "'?", icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete', cancelButtonText: 'Cancel'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
