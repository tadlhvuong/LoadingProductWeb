var Demo = (function () {

    function output(node) {
        var existing = $('#result .croppie-result');
        if (existing.length > 0) {
            existing[0].parentNode.replaceChild(node, existing[0]);
        }
        else {
            $('#result')[0].appendChild(node);
        }
    }

    function popupResult(result) {
        var html;
        if (result.html) {
            html = result.html;
        }
        if (result.src) {
            html = '<img src="' + result.src + '" />';
        }
        swal({
            title: '',
            html: true,
            text: html,
            allowOutsideClick: true
        });
        setTimeout(function () {
            $('.sweet-alert').css('margin', function () {
                var top = -1 * ($(this).height() / 2),
                    left = -1 * ($(this).width() / 2);

                return top + 'px 0 0 ' + left + 'px';
            });
        }, 1);
    }

    function demoUpload() {
        var $uploadCrop;
        function readFile(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                //console.log(input.files[0].name); //name_image
                reader.onload = function (e) {
                    $('.upload-demo').addClass('ready');
                    $uploadCrop.croppie('bind', {
                        url: e.target.result
                    }).then(function () {
                        console.log('jQuery bind complete');
                    });

                };

                reader.readAsDataURL(input.files[0]);
            }
            else {
                swal("Sorry - you're browser doesn't support the FileReader API");
            }
        }


        $uploadCrop = $('#upload-demo').croppie({
            viewport: {
                width: 300,
                height: 300
            },
            enableExif: true
        });

        $('#uploadFile').on('change', function () { readFile(this); });
        $('.upload-result').on('click', function (ev) {

            $uploadCrop.croppie('result', {
                type: 'canvas',
                size: 'viewport'
            }).then(function (resp) {
                $('.cr-image').attr("src", resp);
                $('.cr-image').css("transform", "translate3d(10px, 10px, 0px) scale(1)");
                $('#avatar').attr("src", resp);
                $.ajax({
                    url: '/Admin/Media/FileUploadCrop',
                    type: "POST",
                    data: { "fileData": resp },
                    success: function (data) {
                        console.log(data.initialPreview[0]);

                        document.getElementById('Image').value = (data.initialPreview[0])
                        //$('#modal-croppie').modal('toggle');
                        //if (data.code === 200) {
                        //    html = '<img src="' + data.image + '" height="250" width="250" />';
                        //    $('#image').val(data.image);
                        //}
                        //else {
                        //    html = '<p style="color: red">Image Upload Fail. Pleast try again</p>'
                        //}
                        //$("#croppie-img").html(html);

                    },
                    fail: function (data) {
                    }
                });

                //popupResult({
                //    src: resp
                //});
            });
        });
    }
    function bindNavigation() {
        var $html = $('html');
        $('nav a').on('click', function (ev) {
            var lnk = $(ev.currentTarget),
                href = lnk.attr('href'),
                targetTop = $('a[name=' + href.substring(1) + ']').offset().top;

            $html.animate({ scrollTop: targetTop });
            ev.preventDefault();
        });
    }

    function init() {
        demoUpload();
    }

    return {
        init: init
    };
})();


// Full version of `log` that:
//  * Prevents errors on console methods when no console present.
//  * Exposes a global 'log' function that preserves line numbering and formatting.
(function () {
    var method;
    var noop = function () { };
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }


    if (Function.prototype.bind) {
        window.log = Function.prototype.bind.call(console.log, console);
    }
    else {
        window.log = function () {
            Function.prototype.apply.call(console.log, console, arguments);
        };
    }
})();
