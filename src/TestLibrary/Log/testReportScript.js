function ToggleItem(type) {
    var traces = document.getElementsByClassName(type);
    if (traces[0].style.visibility != 'visible') {
        for (var i = 0; i < traces.length; i++) {
            traces[i].style.visibility = "visible";
            traces[i].style.position = "relative";
        }
    }
    else {

        for (var i = 0; i < traces.length; i++) {
            traces[i].style.visibility = "hidden";
            //traces[i].style.position = "relative";
            traces[i].style.position = "absolute";
        }
    }
}
function ToggleItems(type) {
    if (type.constructor === Array) {
        for (var i = 0; i < type.length; i++) {
            ToggleItem(type[i]);
        }
    }
    else
    {
        ToggleItem(type)
    }
}

function ExpanseAll() {
    var mainTree = document.getElementsByClassName('css-treeview');
    if (mainTree != null)
    {
        var liList = mainTree[0].getElementsByTagName('li');
        for(var i=0;i <= liList.length; i++)
        {
            var checkBox = liList[i].getElementsByTagName('input');
            if (checkBox!=null && checkBox.length > 0)
            {
                for (var j = 0; j < checkBox.length; j++) {
                    if (checkBox[j].type == 'checkbox')
                    {
                        checkBox[j].checked = 'checked';
                    }
                }
            }
        }
    }
}

function CollapseAll() {
    var mainTree = document.getElementsByClassName('css-treeview');
    if (mainTree != null) {
        var liList = mainTree[0].getElementsByTagName('li');
        for (var i = 0; i < liList.length; i++) {
            var checkBox = liList[i].getElementsByTagName('input');
            if (checkBox != null && checkBox.length > 0) {
                for (var j = 0; j < checkBox.length; j++) {
                    if (checkBox[j].type == 'checkbox') {
                        checkBox[j].checked = null;
                    }
                }
            }
        }
    }
}

function GoToTestCase(testCaseElementId) {
    var checkBox = document.getElementById(testCaseElementId)
    if (checkBox != null) {
        if (checkBox.type == 'checkbox') {
            checkBox.checked = 'checked';
        }
        if (checkBox.scrollIntoView) {
            checkBox.scrollIntoView(true);
        }
    }
}