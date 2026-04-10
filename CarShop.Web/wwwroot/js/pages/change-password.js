(function () {
    var newPwd     = document.getElementById('newPwd');
    var confirmPwd = document.getElementById('confirmPwd');
    if (!newPwd || !confirmPwd) return;

    newPwd.addEventListener('input', function () {
        var p    = this.value;
        var wrap = document.getElementById('strengthWrap');
        if (!p) { wrap.style.display = 'none'; return; }
        wrap.style.display = 'block';
        var score  = [p.length >= 6, /\d/.test(p), /[a-z]/.test(p), /[A-Z]/.test(p), /\W/.test(p)].filter(Boolean).length;
        var levels = [
            { pct: 20,  cls: 'bg-danger',  text: 'Very Weak' },
            { pct: 40,  cls: 'bg-warning', text: 'Weak' },
            { pct: 60,  cls: 'bg-info',    text: 'Fair' },
            { pct: 80,  cls: 'bg-primary', text: 'Strong' },
            { pct: 100, cls: 'bg-success', text: 'Very Strong' }
        ];
        var lvl = levels[score - 1] || levels[0];
        var bar = document.getElementById('strengthBar');
        bar.style.width = lvl.pct + '%';
        bar.className   = 'progress-bar ' + lvl.cls;
        document.getElementById('strengthLabel').textContent = lvl.text;
    });

    confirmPwd.addEventListener('input', function () {
        var match = this.value === newPwd.value;
        document.getElementById('matchError').style.display = match ? 'none' : 'block';
    });
})();
