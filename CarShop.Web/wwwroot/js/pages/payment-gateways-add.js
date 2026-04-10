function onSlugChange(slug) {
    var schema = window._gatewaySchema;
    var section = document.getElementById('configSection');
    var container = document.getElementById('configFields');
    container.innerHTML = '';
    var fields = schema ? schema[slug] : null;
    if (!fields || fields.length === 0) { section.style.display = 'none'; return; }
    fields.forEach(function (f) {
        var div = document.createElement('div');
        div.className = 'mb-3';
        var inputType = f.IsSecret ? 'password' : 'text';
        div.innerHTML =
            '<label class="form-label">' + f.Label +
            (f.IsSecret ? ' <span class="badge bg-warning text-dark" style="font-size:.65rem;">Secret</span>' : '') +
            '</label>' +
            '<input type="' + inputType + '" name="configFields[' + f.Key + ']"' +
            ' class="form-control" placeholder="' + f.Placeholder + '" autocomplete="new-password" />';
        container.appendChild(div);
    });
    section.style.display = '';
}
