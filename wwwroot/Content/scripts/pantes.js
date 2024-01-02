(function () {
    var mainScriptEl = document.getElementById('main');
    if (!mainScriptEl) return;
    var preEl = document.createElement('pre');
    var codeEl = document.createElement('code');
    codeEl.innerHTML = mainScriptEl.innerHTML;
    codeEl.className = 'language-javascript';
    preEl.appendChild(codeEl);
    /*document.getElementById('bd-wrapper').appendChild(preEl);*/
})();