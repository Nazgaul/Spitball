import Vue from 'vue';
import { LanguageService } from '../services/language/languageService';

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

Vue.filter('ellipsis', function (value, characters, detailedView) {
    value = value || '';
    if (value.length <= characters || detailedView) {
        return value;
    } else {
        return value.substr(0, characters) + '...';

    }
});

Vue.filter('bolder', function (value, query) {
    if (query.length) {
        query.map((item) => {
            value = value.replace(item, '<span class="bolder">' + item + '</span>');
        });
    }
    return value;
});

Vue.filter('fixedPoints', function (value) {
    if (!value) return 0;
    if (value.toString().indexOf('.') === -1) return value;
    return parseFloat(value).toFixed(2);
});

Vue.filter('dollarVal', function (value) {
    if (!value) return 0;
    return parseFloat(value / 100).toFixed(2);
});


// 10/12/2018
Vue.filter('dateFromISO', function (value) {
    let d = new Date(value);
    //return load if no data
    if (!value) {
        return LanguageService.getValueByKey('wallet_Loading');
    }
    return `${d.getUTCMonth() + 1}/${d.getUTCDate()}/${d.getUTCFullYear()}`;
});