function parseAllData() {
    var resArr = [];
    var bigArr = $(".card__content");
    for (var i = 0; i < bigArr.length; i++) {
        var coolItem = $(bigArr[i]);
        var arr = coolItem.find(".first-letter");
        var str = arr[0].innerText;
        str += "###" + arr.last()[0].innerText;
        var bottomPanel = coolItem.find(".query-item__info");
        str += "###" + bottomPanel[0].innerText;
        //console.log(str);
        resArr.push(str);
    }

    var resStr = "";
    for (var i = 0; i < resArr.length; i++) {
        resStr += resArr[i] + "^^^";
    }
    return resStr;
}
parseAllData();