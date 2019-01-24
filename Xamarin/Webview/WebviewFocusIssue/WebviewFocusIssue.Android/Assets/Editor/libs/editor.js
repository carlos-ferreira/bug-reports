window.onbeforeunload = function() {
    return false;
};

var title = document.getElementById('title');
var editor = document.getElementById('content');
var editor_container = document.getElementById('editor_container');
var editor_html_container = document.getElementById('htmlEditor');

var BPEditor = {
    title: '',
    content: '',
    selection: '',
    isHTMLMode: false,
    initialize: function (titlePH, contentPH) {
        title = document.getElementById('title');
        editor = document.getElementById('content');
        editor_container = document.getElementById('editor_container');
        editor_html_container = document.getElementById('htmlEditor');
        title.setAttribute('placeholder', titlePH);
        editor.setAttribute('placeholder', contentPH);
        title.addEventListener("keyup", this.onKeyUp);
        editor.addEventListener("keydown", this.onEnterKeyPressed);
        document.execCommand('defaultParagraphSeparator', false, 'p');
    },
    onEnterKeyPressed: function (e) {

        if (e.keyCode === 13) {

            if (BPEditor.hasList()) {
                return true;
            }

            if (BPEditor.hasBlockQuote()) {
                e.preventDefault();
                document.execCommand('insertHTML', false, '<br />');
                return true;
            }
        }

        return true;

    },
    onKeyUp: function () {
        BPEditor.title = title.value;
        BPEditor.content = editor.innerHTML;
    },
    focus: function () {
        editor.focus();
    },
    executeCommand: function (command, value) {
        document.execCommand(command, false, value);
        this.focus();
    },
    toggleBold: function () {
        this.executeCommand('bold');
    }
};





