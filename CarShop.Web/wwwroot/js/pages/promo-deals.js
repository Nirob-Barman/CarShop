function copyDealCode(btn) {
    var code = btn.dataset.code;
    navigator.clipboard.writeText(code).then(function () {
        var orig = btn.innerHTML;
        btn.innerHTML = '✅ Copied!';
        btn.classList.replace('btn-outline-dark', 'btn-success');
        setTimeout(function () {
            btn.innerHTML = orig;
            btn.classList.replace('btn-success', 'btn-outline-dark');
        }, 2000);
    });
}
