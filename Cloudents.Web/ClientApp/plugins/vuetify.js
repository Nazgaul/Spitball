import Vue from 'vue';
import Vuetify from 'vuetify/lib';


Vue.use(Vuetify);

import he from '../../node_modules/vuetify/src/locale/he.ts';
import en from '../../node_modules/vuetify/src/locale/en.ts';

const opts = {
    lang: {
        locales: { en, he },
        current: global.lang,
      },
      theme: { disable: true }
}

export default new Vuetify(opts)