import Vue from 'vue';
import Vuetify, { VBtn, VImg, VAvatar,VContainer, VDialog, VLayout, VFlex, VIcon, VRow, VCol } from 'vuetify/lib';
import { Touch } from 'vuetify/lib/directives'


Vue.use(Vuetify, {
  directives: {
    Touch
  },
  components: { VBtn, VImg, VAvatar, VDialog, VLayout, VIcon,VFlex,VRow,VCol,VContainer }
});

import he from '../../node_modules/vuetify/src/locale/he.ts';
import en from '../../node_modules/vuetify/src/locale/en.ts';

//https://vuetifyjs.com/en/customization/icons/
const MY_ICONS = {
//  complete: '...',
 // cancel: '...',
 // close: '...',
 // delete: '...', // delete (e.g. v-chip close)
  // clear: '...',
  // success: '...',
  // info: '...',
  // warning: '...',
  // error: '...',
  // prev: '...',
  // next: '...',
  // checkboxOn: '...',
  // checkboxOff: '...',
  // checkboxIndeterminate: '...',
 // delimiter: '...', // for carousel
  // sort: '...',
  // expand: '...',
  // menu: '...',
  // subgroup: '...',
  // dropdown: '...',
  // radioOn: '...',
  // radioOff: '...',
  // edit: '...',
  ratingEmpty: 'sbf-star-rating-empty',
  ratingFull: 'sbf-star-rating-full',
  ratingHalf: 'sbf-star-rating-half',
  // loading: '...',
  // first: '...',
  // last: '...',
  // unfold: '...',
  // file: '...',
}

const opts = {
  lang: {
    locales: { en, he },
    current: global.lang,
  },
  theme: {
    theme : 'disabled'
  },
  icons: {
    values: MY_ICONS
  }
}

export default new Vuetify(opts)