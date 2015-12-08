var s = document.getElementsByTagName('script'),
    e = s[s.length - 1].parentElement,
    p1 = e.previousSibling,
    p2 = p1 ? p1.previousSibling : p1,
    f = p1.tagName ? p1 : p2;
f.innerHTML && f.innerHTML.replace(/^\s+|\s+$/g, '').length > 0 || (
    e.removeAttribute('style'),
    f.setAttribute('style', 'display:none;')
);