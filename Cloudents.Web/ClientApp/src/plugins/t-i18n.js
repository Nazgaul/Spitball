import Vue from 'vue'
import VueI18n from 'vue-i18n'
//import messages from '../locales/en.json'
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


const numberFormats = {
  'en-US': {
    currency: {
      style: 'currency', currency: 'USD', minimumFractionDigits: 0, maximumFractionDigits: 0
    },
  },
  // 'en-IL': {
  //   currency: {
  //     style: 'currency', currency: 'ILS', minimumFractionDigits: 0, maximumFractionDigits: 0
  //   },
  // },
  // 'he-IL': {
  //   currency: {
  //     style: 'currency', currency: 'ILS', minimumFractionDigits: 0, maximumFractionDigits: 0
  //   },
  // },
  'he': {
    currency: {
      style: 'currency', currency: 'ILS', minimumFractionDigits: 0, maximumFractionDigits: 0
    },
  },
  'en-IN': {
    currency: {
      style: 'currency', currency: 'INR', minimumFractionDigits: 0, maximumFractionDigits: 0
    },
  }
}

const dateTimeFormats = {
  'en': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    tableDate: {
      month: 'short', day: 'numeric'
    },
    justDay: {
      weekday: 'short'
    },
    calendarDesktop: {
      weekday: 'short'
    },
    calendarMobile: {
      weekday: 'narrow'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'long', hour: 'numeric', minute: 'numeric'
    }
  },
  'en-IL': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    tableDate: {
      month: 'short', day: 'numeric'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'long', hour: 'numeric', minute: 'numeric'
    }
  },
  'he-IL': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    tableDate: {
      month: 'short', day: 'numeric'
    },
    calendarDesktop: {
      weekday: 'short'
    },
    calendarMobile: {
      weekday: 'narrow'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'long', hour: 'numeric', minute: 'numeric'
    }
  },
  'en-IN': {
    short: {
      year: 'numeric', month: 'short', day: 'numeric'
    },
    tableDate: {
      month: 'short', day: 'numeric'
    },
    long: {
      year: 'numeric', month: 'short', day: 'numeric',
      weekday: 'long', hour: 'numeric', minute: 'numeric'
    }
  }
}
const lang = `${global.lang}-${global.country}`;

const supportedLanguges = ['en', 'en-US', 'en-IN', 'he'];

//TODO we can put a loop in here
export const i18n = new VueI18n({
  locale: lang,
  //fallbackLocale: 'en',
  // fallbackLocale: {
  //   'he-IL': 'he',
  //   default: 'en'
  // },
  //messages: messages,
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

export async function loadLanguageAsync() {
  let isFrymo = global.location.hostname.toLowerCase();
  
  // https://www.frymo.com/
  
  let lang = `${global.lang}-${global.country}`;
  // eslint-disable-next-line no-constant-condition
  while (true) {
    if (supportedLanguges.indexOf(lang) !== -1) {
      break;
    }
    let lang2 = lang.split('-')[0];
    if (lang2 == lang) {
      lang = 'en';
      break;
    }
    if(isFrymo === 'www.frymo.com') {
      lang = 'en-IN'
      break
    }
    lang = lang2;

  }

  if (loadedLanguages.includes(lang)) {
    return;
  }

  var messages2;
  try {
    let xxx = await import(/* webpackChunkName: "lang-[request]" */ `../locales/${lang}.json`);
    messages2 = xxx;
    // i18n.setLocaleMessage(lang, messages);
    // oadedLanguages.push(lang);
  } catch (error) {
    console.error("no resource", lang, error);
  }

  //return connectivityModule.http.get(`/Locale${dictionaryType}`).then((dictionary)=>{
 
 // messages = { ...messages,...data};
 //let x = {...messages , ...{}};
  i18n.setLocaleMessage(lang,messages2)

  i18n.locale = lang;
  loadedLanguages.push(lang)
  // .then(({data}) => {

  //   return Promise.resolve(lang)
  //   //return lang;
  // })
  // If the language hasn't been loaded yet
  // return import(/* webpackChunkName: "lang-[request]" */ `@/i18n/messages/${lang}.js`).then(
  //   messages => {
  //     i18n.setLocaleMessage(lang, messages.default)
  //     loadedLanguages.push(lang)
  //     return setI18nLanguage(lang)
  //   }
  // )
}