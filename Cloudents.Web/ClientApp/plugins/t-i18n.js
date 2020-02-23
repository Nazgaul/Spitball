import Vue from 'vue'
import VueI18n from 'vue-i18n'
import axios from 'axios'

Vue.use(VueI18n)

// function loadLocaleMessages () {
//     debugger;
//   const locales = require.context('../', true, /[A-Za-z0-9-_,\s]+\.json$/i)
//   const messages = {}
//   locales.keys().forEach(key => {
//     const matched = key.match(/([A-Za-z0-9-_]+)\./i)
//     if (matched && matched.length > 1) {
//       const locale = matched[1]
//       messages[locale] = locales(key)
//     }
//   })
//   return messages
// }
const lang = `${global.lang}-${global.country}`;


const numberFormats = {
  'en': {
    currency: {
      style: 'currency', currency: 'USD', minimumFractionDigits: 0
    },
  },
  'en-IL': {
    currency: {
      style: 'currency', currency: 'ILS', minimumFractionDigits: 0
    },
  },
  'he-IL': {
    currency: {
      style: 'currency', currency: 'ILS', minimumFractionDigits: 0
    },
  },
  'en-IN': {
    currency: {
      style: 'currency', currency: 'INR', minimumFractionDigits: 0
    },
  }
}

const dateTimeFormats = {
  'en': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    justDay: {
      weekday:'short'
    },
    calendarDesktop: {
      weekday:'short'
    },
    calendarMobile: {
      weekday:'narrow'
    }
  },
  'en-IL': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
  },
  'he-IL': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    calendarDesktop: {
      weekday:'short'
    },
    calendarMobile: {
      weekday:'narrow'
    }
  },
  'en-IN': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
  }
}


export const i18n =  new VueI18n({
  locale:  lang,
  fallbackLocale: 'en',
  messages : {},
  numberFormats,
  dateTimeFormats
  //messages: loadLocaleMessages()
})


const loadedLanguages = [] // our default language that is preloaded

// function setI18nLanguage (lang) {
//   i18n.locale = lang
//   axios.defaults.headers.common['Accept-Language'] = lang
//   document.querySelector('html').setAttribute('lang', lang)
//   return lang
// }

export function loadLanguageAsync() {
  // If the same language
  // if (i18n.locale === lang) {
  //   return Promise.resolve()
  // }

  // If the language was already loaded
  if (loadedLanguages.includes(lang)) {
    return Promise.resolve()
  }

//  let dictionaryType = `?v=${global.version}&culture=${global.lang}-${global.country}`;
  // if(!!type){
  //     //version is for anti caching ability
  //     dictionaryType += `&resource=${type}`;
  // }else{
  //     dictionaryType += '';
  // }
  //return connectivityModule.http.get(`/Locale${dictionaryType}`).then((dictionary)=>{
  return axios.get('/locale', {
    params : {
      v : global.version,
      culture : lang
    }
  }).then(({data}) => {
    i18n.mergeLocaleMessage(lang, data)
    loadedLanguages.push(lang)
    return Promise.resolve(lang)
    //return lang;
  })
  // If the language hasn't been loaded yet
  // return import(/* webpackChunkName: "lang-[request]" */ `@/i18n/messages/${lang}.js`).then(
  //   messages => {
  //     i18n.setLocaleMessage(lang, messages.default)
  //     loadedLanguages.push(lang)
  //     return setI18nLanguage(lang)
  //   }
  // )
}