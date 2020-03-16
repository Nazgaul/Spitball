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