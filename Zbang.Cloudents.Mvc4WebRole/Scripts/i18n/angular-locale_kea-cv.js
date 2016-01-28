'use strict';
angular.module("ngLocale", [], ["$provide", function($provide) {
var PLURAL_CATEGORY = {ZERO: "zero", ONE: "one", TWO: "two", FEW: "few", MANY: "many", OTHER: "other"};
function getDecimals(n) {
  n = n + '';
  var i = n.indexOf('.');
  return (i == -1) ? 0 : n.length - i - 1;
}

function getVF(n, opt_precision) {
  var v = opt_precision;

  if (undefined === v) {
    v = Math.min(getDecimals(n), 3);
  }

  var base = Math.pow(10, v);
  var f = ((n * base) | 0) % base;
  return {v: v, f: f};
}

$provide.value("$locale", {
  "DATETIME_FORMATS": {
    "AMPMS": [
      "am",
      "pm"
    ],
    "DAY": [
      "dumingu",
      "sigunda-fera",
      "tersa-fera",
      "kuarta-fera",
      "kinta-fera",
      "sesta-fera",
      "sabadu"
    ],
    "ERANAMES": [
      "Antis di Kristu",
      "Dispos di Kristu"
    ],
    "ERAS": [
      "AK",
      "DK"
    ],
    "FIRSTDAYOFWEEK": 0,
    "MONTH": [
      "Janeru",
      "Febreru",
      "Marsu",
      "Abril",
      "Maiu",
      "Junhu",
      "Julhu",
      "Agostu",
      "Setenbru",
      "Otubru",
      "Nuvenbru",
      "Dizenbru"
    ],
    "SHORTDAY": [
      "dum",
      "sig",
      "ter",
      "kua",
      "kin",
      "ses",
      "sab"
    ],
    "SHORTMONTH": [
      "Jan",
      "Feb",
      "Mar",
      "Abr",
      "Mai",
      "Jun",
      "Jul",
      "Ago",
      "Set",
      "Otu",
      "Nuv",
      "Diz"
    ],
    "STANDALONEMONTH": [
      "Janeru",
      "Febreru",
      "Marsu",
      "Abril",
      "Maiu",
      "Junhu",
      "Julhu",
      "Agostu",
      "Setenbru",
      "Otubru",
      "Nuvenbru",
      "Dizenbru"
    ],
    "WEEKENDRANGE": [
      5,
      6
    ],
    "fullDate": "EEEE, d 'di' MMMM 'di' y",
    "longDate": "d 'di' MMMM 'di' y",
    "medium": "d MMM y HH:mm:ss",
    "mediumDate": "d MMM y",
    "mediumTime": "HH:mm:ss",
    "short": "d/M/y HH:mm",
    "shortDate": "d/M/y",
    "shortTime": "HH:mm"
  },
  "NUMBER_FORMATS": {
    "CURRENCY_SYM": "CVE",
    "DECIMAL_SEP": ",",
    "GROUP_SEP": "\u00a0",
    "PATTERNS": [
      {
        "gSize": 3,
        "lgSize": 3,
        "maxFrac": 3,
        "minFrac": 0,
        "minInt": 1,
        "negPre": "-",
        "negSuf": "",
        "posPre": "",
        "posSuf": ""
      },
      {
        "gSize": 3,
        "lgSize": 3,
        "maxFrac": 2,
        "minFrac": 2,
        "minInt": 1,
        "negPre": "-",
        "negSuf": "\u00a0\u00a4",
        "posPre": "",
        "posSuf": "\u00a0\u00a4"
      }
    ]
  },
  "id": "kea-cv",
  "pluralCat": function(n, opt_precision) {  var i = n | 0;  var vf = getVF(n, opt_precision);  if (i == 1 && vf.v == 0) {    return PLURAL_CATEGORY.ONE;  }  return PLURAL_CATEGORY.OTHER;}
});
}]);
