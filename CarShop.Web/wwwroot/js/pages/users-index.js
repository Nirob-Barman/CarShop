function confirmUnban(btn, email) {
    Swal.fire({
        title: 'Unban user?', text: 'Unban ' + email + '?', icon: 'question',
        showCancelButton: true, confirmButtonColor: '#198754', cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, unban', cancelButtonText: 'Cancel'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}

function confirmBan(btn, email) {
    Swal.fire({
        title: 'Ban user?', text: 'Ban ' + email + '?', icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, ban', cancelButtonText: 'Cancel'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
