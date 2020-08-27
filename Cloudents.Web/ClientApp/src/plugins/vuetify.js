import Vue from 'vue';
import Vuetify, { VBtn, VImg, VAvatar,VContainer, VDialog, VLayout, VFlex, VIcon, VRow, VCol } from 'vuetify/lib';
import { Touch, ClickOutside} from 'vuetify/lib/directives'


Vue.use(Vuetify, {
  directives: {
    Touch,
    ClickOutside
  },
  components: { VBtn, VImg, VAvatar, VDialog, VLayout, VIcon,VFlex,VRow,VCol,VContainer }
});

import he from '../../node_modules/vuetify/src/locale/he.ts';
import en from '../../node_modules/vuetify/src/locale/en.ts';

//https://vuetifyjs.com/en/customization/icons/
const MY_ICONS = {
//  complete: '...',
 // cancel: '...',
 close: 'sbf-close',
 // delete: '...', // delete (e.g. v-chip close)
  // clear: '...',
  // success: '...',
  // info: '...',
  dotMenu: 'sbf-3-dot',
  // warning: '...',
  // error: '...',
  lock: 'sbf-lock',
  prev: 'sbf-arrow-left-carousel',
  next: 'sbf-arrow-right-carousel',
   checkboxOn: 'sbf-check-box-done',
   checkboxOff: 'sbf-check-box-un',
  // checkboxIndeterminate: '...',
 // delimiter: '...', // for carousel
  sort: 'sbf-arrow-up',
  // expand: '...',
  // menu: '...',
  // subgroup: '...',
  dropdown: 'sbf-arrow-down',
  radioOn: 'sbf-radioOn',
  radioOff: 'sbf-radioOff',
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
  // theme: {
  //   theme : 'disabled'
  // },
  icons: {
    values: MY_ICONS
  },
  rtl: document.dir === "rtl"//global.isRtl
}

export default new Vuetify(opts)