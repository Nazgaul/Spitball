import Vue from 'vue';

Vue.filter('capitalize', function (value) {
    if (!value) return '';

    value = value.toString();

    let values = value.split("/");
    for (let v in values) {
        let tempVal = values[v];
        values[v] = tempVal.charAt(0).toUpperCase() + tempVal.slice(1);
    }
    return values.join(" ");
});