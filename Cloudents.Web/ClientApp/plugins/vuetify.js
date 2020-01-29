import Vue from 'vue';
import Vuetify, { VBtn, VImage, VAvatar, VDialog, VGrid, VIcon } from 'vuetify/lib';
import { Touch } from 'vuetify/lib/directives'


Vue.use(Vuetify, {
  directives: {
    Touch
  },
  components: { VBtn, VImage , VAvatar, VDialog, VGrid, VIcon }
});

import he from '../../node_modules/vuetify/src/locale/he.ts';
import en from '../../node_modules/vuetify/src/locale/en.ts';

const opts = {
  lang: {
    locales: { en, he },
    current: global.lang,
  },
  theme: {
    theme : 'disabled'
  }
}

export default new Vuetify(opts)