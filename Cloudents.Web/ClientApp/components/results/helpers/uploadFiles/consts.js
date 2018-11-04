import { LanguageService } from "../../../../services/language/languageService";

export let documentTypes = [
    {
        id: 'lecture',
        title: LanguageService.getValueByKey("upload_files_type_lecture"),
        icon:"sbf-lecture-note",

    },
    {
        id: 'textbook',
        title:  LanguageService.getValueByKey("upload_files_type_textbook"),
        icon:"sbf-textbook-note"
    },
    {
        id: 'exam',
        title: LanguageService.getValueByKey("upload_files_type_exam"),
        icon:"sbf-exam-note"
    },
];

export let currencyValidator = {
    format: function (number) {
        return (Math.trunc(number * 1000000000000) / 1000000000000).toFixed(2);
    },
    parse: function (newString, oldNumber) {
        var CleanParse = function (value) {
            return { value: value }
        }
        var CurrencyWarning = function (warning, value) {
            return {
                warning: warning,
                value: value,
                attempt: newString
            }
        }
        var NotAValidDollarAmountWarning = function (value) {
            return new CurrencyWarning(newString + ' is not a valid dollar amount', value)
        }
        var AutomaticConversionWarning = function (value) {
            return new CurrencyWarning(newString + ' was automatically converted to ' + value, value)
        }

        var newNumber = Number(newString)
        var indexOfDot = newString.indexOf('.')
        var indexOfE = newString.indexOf('e')

        if (isNaN(newNumber)) {
            if (
                indexOfDot === -1 &&
                indexOfE > 0 &&
                indexOfE === newString.length - 1 &&
                Number(newString.slice(0, indexOfE)) !== 0
            ) {
                return new CleanParse(oldNumber)
            } else {
                return new NotAValidDollarAmountWarning(oldNumber)
            }
        }

        var newCurrencyString = currencyValidator.format(newNumber)
        var newCurrencyNumber = Number(newCurrencyString)

        if (newCurrencyNumber === newNumber) {
            if (indexOfE !== -1 && indexOfE === newString.length - 2) {
                return new AutomaticConversionWarning(newNumber)
            } else {
                return new CleanParse(newNumber)
            }
        } else {
            return new NotAValidDollarAmountWarning(
                newNumber > newCurrencyNumber
                    ? newCurrencyNumber
                    : oldNumber
            )
        }
    }
}