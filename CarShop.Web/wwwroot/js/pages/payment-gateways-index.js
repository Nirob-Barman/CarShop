function confirmDelete(btn, name) {
    Swal.fire({
        title: 'Delete gateway?', text: "Delete '" + name + "'? This cannot be undone.", icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete', cancelButtonText: 'Cancel'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
