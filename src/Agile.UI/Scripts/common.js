var App = window.App || {};
(function ($) {
    var app = window.App
    if (!window.console) { window.console = { log: function () { } }; }

    var guid = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }


    // ajax interceptors
    $(document).ajaxStart(function (event, request, settings) {

    })
    $(document).ajaxSend(function (event, request, settings) {
        if (!settings.noMaskLoading) {
            $.mask();
        }
    });
    $(document).ajaxSuccess(function (event, request, settings) {
        console.log('in ajaxSuccess')
        if (request.responseJSON && request.responseJSON.Success) {
            if (settings.onSuccess && $.isFunction(settings.onSuccess)) {
                settings.onSuccess(request.responseJSON);
            } else {

            }
        } else if (request.responseJOSN && request.responseJSON.Message) {
            $.err('发生错误：' + request.responseJSON.Message)
            if (settings.onError && $.isFunction(settings.onError)) {
                if (request.responseJSON) {
                    settings.onError(request.responseJSON)
                } else {
                    settings.onError(request.responseText)
                }
            }
        }
    })
    $(document).ajaxError(function (event, request, settings) {
        console.log('in ajaxError' + request)
        switch (request.status) {
            case 500:
                if (request.responseJSON && request.responseJSON.Message) {
                    $.err("发生错误：" + request.responseJSON.Message)
                } else {
                    $.err("发生错误")
                }
                if (settings.onError && $.isFunction(settings.onError)) {
                    if (request.responseJSON) {
                        settings.onError(request.responseJSON)
                    } else {
                        settings.onError(request.responseText)
                    }
                }
                break;
            case 401:
                $.err('无权限操作，请尝试刷新页面或者换帐号登录')
                break;
            case 405:
                $.err('您已掉线，3秒后自动重新登录')
                setTimeout(function () {
                    window.location.reload()
                }, 3000)
                break;
            case 404:
                $.err("页面未找到，请尝试刷新页面")
                break;
            default:
                $.err('发生未知错误,请尝试刷新页面或者重新登录')
                break;
        }
    })
    $(document).ajaxComplete(function (event, request, settings) {
        if (!settings.noMaskLoading) {
            $.unmask()
        }
    })
    $(document).ajaxStop(function (event, request, settings) {
        console.log('in ajaxStop')

    })

    // form extensions
    $.fn.extend({
        reset: function () {
            var form = $(this);
            form.find('input')
			 .not(':button, :submit, :reset, :hidden')
			 .val('')
			 .removeAttr('checked')
			 .removeAttr('selected');
            //清空下拉框。如果下拉框第一个option的值为空或者具有data-select-default属性，那么设置为这个值
            form.find('select').each(function (idx, me) {
                var select = $(me);
                var options = select.find('option');
                if (options && options.length && options[0].value === "") {
                    select.val(options[0].value);
                } else {
                    var defaultOption = select.find('[data-select-default]');
                    if (defaultOption && defaultOption.length) {
                        select.val(defaultOption[0].value);
                    }
                }

            })
        },
        setReadOnly: function () {
            var form = $(this);
            form.find('input[type="text"]').each(function (idx, item) {
                $(item).attr('readOnly', "readOnly")
            })
            form.find('textarea').each(function (idx, item) {
                $(item).attr('readOnly', 'readOnly');
            })
            form.find('input[type="file"]').each(function (idx, item) {
                $(item).attr('disabled', "disabled");
            })
            form.find('select').attr('disabled', "disabled")

        },
        toJson: function () {
            var o = {};
            var a = this.serializeArray();
            $.each(a, function () {
                if (o[this.name]) {
                    if (!o[this.name].push) {
                        o[this.name] = [o[this.name]];
                    }
                    o[this.name].push(this.value || '');
                } else {
                    o[this.name] = this.value || '';
                }
            });
            return o;
        }
    })

    $("[role='clear']").on('click', function (e) {
        e.preventDefault();
        var form = $(this).closest("form");
        form.reset();
    });
    $("[role='export']").on("click", function (e) {
        e.preventDefault();
        var form = $(this).closest("form");
        form.append("<input type='hidden' name='__report' value='true'>");
        form.submit();
        form.find("[name='__report']").remove();
    })

    //表单验证区域
    $('form').validationEngine('attach', {
        maxErrorsPerField: 1,
        autoHidePrompt: true,
        autoHideDelay: 2000,
        promptPosition: "bottomRight"
    });

    $('form.async').on('submit', function () {
        var me = this;
        if (!$(me).validationEngine('validate')) {
            return false;
        }
        if ($(me).triggerHandler('beforeSubmit') === false) {
            return false;
        }
        if ($(me).hasClass("json")) {
            $.ajax({
                type: 'post',
                url: $(me).attr('action'),
                dataType: "json",
                data: JSON.stringify($(me).toJson()),
                contentType: "application/json",
                onSuccess: function (result) {
                    $.msg("保存成功")
                    $(me).triggerHandler('success', [result])
                },
                onError: function (result) {
                    $(me).triggerHandler('error', [result])
                }
            })
        } else {
            $.ajax({
                type: 'post',
                url: $(me).attr('action'),
                data: $(me).serialize(),
                onSuccess: function (result) {
                    $.msg("保存成功")
                    $(me).triggerHandler('success', [result])
                },
                onError: function (result) {
                    $(me).triggerHandler('error', [result])
                }
            })
        }

        return false;
    })
    //表单验证自动把label加上*号
    $('form').find('[class*=required]').each(function (idx, item) {
        var label = $(item).closest('.form-group').find('label:first');
        //检查是否是label

        label.html(label.html() + '<strong class="text-danger" title="必填">*</strong>');

    })

    //date && datetime picker

    $('input.datetime').each(function () {
        var me = this;

        var min = $(me).data('min-date');
        var max = $(me).data('max-date');
        var minIsMoment = moment(min).isValid();
        var maxIsMoment = moment(max).isValid();
        var minIsSelector = $(min).length === 1;
        var maxIsSelector = $(max).length === 1;
        var minDate = '';
        var maxDate = '';
        if (minIsMoment) {
            minDate = min;
        } else if (minIsSelector) {
            var minId = $(min).attr('id')
            if (!minId) {
                minId = guid()
                $(min).attr('id', minId)
            }
            minDate = "#F{$dp.$D(\'" + minId + "\')}"
        }
        if (maxIsMoment) {
            maxDate = max;
        } else if (maxIsSelector) {
            var maxId = $(max).attr('id')
            if (!maxId) {
                maxId = guid()
                $(max).attr('id', maxId)
            }
            maxDate = "#F{$dp.$D(\'" + maxId + "\')}"
        }
        $(me).on('focus', function () {
            WdatePicker({ el: me, dateFmt: 'yyyy-MM-dd HH:mm:ss', minDate: minDate, maxDate: maxDate });
        })

    })

    $('input.date').each(function () {
        var me = this;

        var min = $(me).data('min-date');
        var max = $(me).data('max-date');
        var minIsMoment = moment(min).isValid();
        var maxIsMoment = moment(max).isValid();
        var minIsSelector = $(min).length === 1;
        var maxIsSelector = $(max).length === 1;
        var minDate = '';
        var maxDate = '';
        if (minIsMoment) {
            minDate = min;
        } else if (minIsSelector) {
            var minId = $(min).attr('id')
            if (!minId) {
                minId = guid()
                $(min).attr('id', minId)
            }
            minDate = "#F{$dp.$D(\'" + minId + "\')}"
        }
        if (maxIsMoment) {
            maxDate = max;
        } else if (maxIsSelector) {
            var maxId = $(max).attr('id')
            if (!maxId) {
                maxId = guid()
                $(max).attr('id', maxId)
            }
            maxDate = "#F{$dp.$D(\'" + maxId + "\')}"
        }
        $(me).on('focus', function () {
            WdatePicker({ el: me, dateFmt: 'yyyy-MM-dd', minDate: minDate, maxDate: maxDate });
        })
    })

    //提示信息控件

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    $.msg = function (content, timeout) {
        if (!content) return;
        if (!timeout) timeout = 5000;
        toastr.clear();
        toastr.info(content, "", { timeOut: timeout });
    }
    $.err = function (content, timeout) {
        if (!content) return;
        if (!timeout) timeout = 5000;
        toastr.clear();
        toastr.error(content, "", { timeOut: timeout });
    }

    //浏览器图片上传拖拽特效
    $(document).on('dragover', function (e) {
        var dropZone = $('.dropzone'),
        foundDropzone,
        timeout = window.dropZoneTimeout;
        if (!timeout) {
            dropZone.addClass('in');
        }
        else {
            clearTimeout(timeout);
        }
        var found = false,
        node = e.target;

        do {

            if ($(node).hasClass('dropzone')) {
                found = true;
                foundDropzone = $(node);
                break;
            }

            node = node.parentNode;

        } while (node != null);

        dropZone.removeClass('in hover');

        if (found) {
            foundDropzone.addClass('hover');
        }

        window.dropZoneTimeout = setTimeout(function () {
            window.dropZoneTimeout = null;
            dropZone.removeClass('in hover');
        }, 100);
    });

    //上传图片组件
    $('.dropzone').each(function (idx, item) {
        var me = $(item);
        var fileInput = me.find('input[type=file]');
        var formData = {};
        fileInput.fileupload({
            dataType: 'json',
            url: me.data('upload-url'),
            paramName: fileInput.attr('name'),
            dropZone: me,
            disableValidation: false,
            autoUpload: true,
            acceptFileTypes: /(\.|\/)(jpe?g)$/i,
            maxFileSize: 1024 * 1024,
            formData: formData,
            add: function (e, data) {
                var uploadErrors = [];
                var acceptFileTypes = /(\.|\/)(jpe?g)$/i;
                var filename = data.originalFiles[0].name;
                if (!acceptFileTypes.test(filename)) {
                    uploadErrors.push('请上传JPG类型');
                }
                if (data.originalFiles[0]['size'] > 1 * 1024 * 1024) {
                    uploadErrors.push('文件不可以超过1M');
                }

                if (uploadErrors.length > 0) {
                    $.err(uploadErrors.join("\n"));
                } else {
                    if (me.triggerHandler('beforeSubmit') === false) {
                        return;
                    }
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data && data.result) {
                    if (data.result.Success) {
                        me.triggerHandler("success", [data.result])
                        me.find('input[type="hidden"]').val(data.result.Data.FileHandle);
                        me.find('img').attr('src', data.result.Data.Url)
                    }
                }
            }
        })
    })

    //远程调用
    $("button[data-rpc-call],input[type='button'][data-rpc-call],a[data-rpc-call]").on("click", function () {
        var me = $(this);
        var remote = me.data("rpc-call");
        var action = me.data("rpc-call-after");
        if (action !== "reload") {
            action = "none";
        }
        if (remote) {
            $.ajax({
                url: remote,
                type: "post",
                onSuccess: function () {
                    $.msg("保存成功");
                    if (action === "reload") {
                        window.location.reload();
                    }
                }
            })
        }
    })

    //初始化设置select中的值,select 无法使用value属性初始化
    $("select[data-select-value]").each(function (index, me) {
        var obj = $(me);
        var attr = obj.data("select-value");
        if (typeof attr !== typeof undefined && attr !== false) {
            //attr exists, set select value
            if (attr !== "") {
                obj.val(attr);
            }
        }
    })


    //image lazy loading
    $("img.lazy").lazyload();


    //iCheck 控件
    $("input.icheck[type='checkbox'],input.icheck[type='radio']").iCheck({
        checkboxClass: 'icheckbox_minimal',
        radioClass: 'iradio_minimal'
    });

    //tradeId query
    $("a[data-trade-id]").each(function () {
        var me = $(this);
        me.qtip({
            content: {
                text: function (event, api) {
                    $.ajax({
                        url: "/P2P/Interface/GetTrade?id=" + me.data("trade-id"),
                        noMaskLoading: true
                    })
                        .then(function (content) {
                            api.set("content.text", content)
                        }, function (xhr, status, error) {
                            api.set("content.text", status + ": " + error)
                        });
                    return "loading...";
                }
            },
            position: {
                viewport: $(window)
            },
            style: "qtip-wiki",
            show: "click",
            hide: "unfocus"
        });
    })


})(jQuery);



// mask loading
(function ($) {
    /**
	 * Displays loading mask over selected element(s). Accepts both single and multiple selectors.
	 *
	 * @param label Text message that will be displayed on top of the mask besides a spinner (optional). 
	 * 				If not provided only mask will be displayed without a label or a spinner.  	
	 * @param delay Delay in milliseconds before element is masked (optional). If unmask() is called 
	 *              before the delay times out, no mask is displayed. This can be used to prevent unnecessary 
	 *              mask display for quick processes.   	
	 */
    $.fn.mask = function (label, delay) {
        if (label == "") {
            label = "请稍等...";
        }
        $(this).each(function () {
            if (delay !== undefined && delay > 0) {
                var element = $(this);
                element.data("_mask_timeout", setTimeout(function () { $.maskElement(element, label) }, delay));
            } else {
                $.maskElement($(this), label);
            }
        });
    };

    /**
	 * Removes mask from the element(s). Accepts both single and multiple selectors.
	 */
    $.fn.unmask = function () {
        $(this).each(function () {
            $.unmaskElement($(this));
        });
    };

    /**
	 * Checks if a single element is masked. Returns false if mask is delayed or not displayed. 
	 */
    $.fn.isMasked = function () {
        return this.hasClass("masked");
    };

    $.maskElement = function (element, label) {
        if (label == undefined || label == "") {
            label = "请稍等...";
        }
        //if this element has delayed mask scheduled then remove it and display the new one
        if (element.data("_mask_timeout") !== undefined) {
            clearTimeout(element.data("_mask_timeout"));
            element.removeData("_mask_timeout");
        }

        if (element.isMasked()) {
            $.unmaskElement(element);
        }

        if (element.css("position") == "static") {
            element.addClass("masked-relative");
        }

        element.addClass("masked");

        var maskDiv = $('<div class="loadmask"></div>');

        //auto height fix for IE
        if (navigator.userAgent.toLowerCase().indexOf("msie") > -1) {
            maskDiv.height(element.height() + parseInt(element.css("padding-top")) + parseInt(element.css("padding-bottom")));
            maskDiv.width(element.width() + parseInt(element.css("padding-left")) + parseInt(element.css("padding-right")));
        }

        //fix for z-index bug with selects in IE6
        if (navigator.userAgent.toLowerCase().indexOf("msie 6") > -1) {
            element.find("select").addClass("masked-hidden");
        }

        element.append(maskDiv);

        if (label !== undefined) {
            var maskMsgDiv = $('<div class="loadmask-msg" style="display:none;"></div>');
            maskMsgDiv.append('<div>' + label + '</div>');
            element.append(maskMsgDiv);

            //calculate center position
            maskMsgDiv.css("top", Math.round(element.height() / 2 - (maskMsgDiv.height() - parseInt(maskMsgDiv.css("padding-top")) - parseInt(maskMsgDiv.css("padding-bottom"))) / 2) + "px");
            maskMsgDiv.css("left", Math.round(element.width() / 2 - (maskMsgDiv.width() - parseInt(maskMsgDiv.css("padding-left")) - parseInt(maskMsgDiv.css("padding-right"))) / 2) + "px");

            maskMsgDiv.show();
        }

    };

    $.unmaskElement = function (element) {
        //if this element has delayed mask scheduled then remove it
        if (element.data("_mask_timeout") !== undefined) {
            clearTimeout(element.data("_mask_timeout"));
            element.removeData("_mask_timeout");
        }

        element.find(".loadmask-msg,.loadmask").remove();
        element.removeClass("masked");
        element.removeClass("masked-relative");
        element.find("select").removeClass("masked-hidden");
    };

    $.mask = function (msg) {
        $('body').mask(msg);
    }
    $.unmask = function () {
        $('body').unmask();
    }
})(jQuery)

