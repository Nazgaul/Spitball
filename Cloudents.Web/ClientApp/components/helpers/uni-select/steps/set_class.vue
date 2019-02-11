<template>
  <!--<div class="select-university-container set-class">-->
  <div>
    <div class="title-container">
      <div class="first-container">
        <div>
          <v-icon @click="lastStep()" :class="{'rtl': isRtl}">sbf-arrow-back</v-icon>
        </div>
        <div>
          <a class="next-container" @click="nextStep()" v-language:inner>uniSelect_done</a>
        </div>
      </div>
      <div class="select-class-string">
        <span v-language:inner>uniSelect_select_class</span>
      </div>
    </div>
    <div class="explain-container">
      <span v-language:inner>uniSelect_from</span>
      {{schoolName}}
    </div>
    <div class="select-school-container">
      <v-combobox
        v-model="selectedClasses"
        :items="classes"
        :label="classNamePlaceholder"

        :placeholder="placeholderVisible ? classNamePlaceholder : ''"
        solo
        :search-input.sync="search"
        :append-icon="''"
        :clear-icon="'sbf-close'"
        autofocus
        multiple
        chips
        :menu-props="dropDownAlphaHeight"
        :color="`gray`"
        :content-class="'set-class-dropdown-container'"
      >
        <template slot="no-data">
          <v-list-tile v-if="showBox">
            <div class="subheading">
              <span v-language:inner>uniSelect_create</span>
              <v-chip @click="addClass(search, classes)">{{ search }}</v-chip>
            </div>
          </v-list-tile>
          <!-- <v-list-tile>
                        <div class="subheading dark">Show all Classes</div>
          </v-list-tile>-->
        </template>
        <template slot="selection" slot-scope="{ item, parent, selected }">
          <v-chip
            class="chip-style"
            :class="{'dark-chip': !itemInList(item), 'selected': selected}"
          >
            <span class="chip-button">{{!!item.text ? item.text : item}}</span>
            <v-icon class="chip-close" @click="parent.selectItem(item)">sbf-close</v-icon>
          </v-chip>
        </template>
        <template style="background-color:black" slot="item" slot-scope="{ index, item }">
          <v-list-tile-content style="max-width:385px;">
            <span v-html="$options.filters.boldText(item.text, search)">{{ item.text }}</span>
          </v-list-tile-content>
        </template>
      </v-combobox>
    </div>
  </div>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import { LanguageService } from "../../../../services/language/languageService";
import debounce from "lodash/debounce";

export default {
  props: {
    fnMethods: {
      required: true,
      type: Object
    },
    enumSteps: {
      required: true,
      type: Object
    }
  },
  data() {
    return {
      search: "",
      classNamePlaceholder: LanguageService.getValueByKey(
        "uniSelect_type_class_name_placeholder"
      ),
      isRtl: global.isRtl,
      global: global
    };
  },
  watch: {
    search: debounce(function(val) {
        let searchVal;
        if (!!val) {
        searchVal = val.trim();
        if (searchVal.length >= 3) {
          this.updateClasses(searchVal);
        }
      }
    }, 500)
  },
  computed: {
    ...mapGetters(["getSelectedClasses"]),
    dropDownAlphaHeight() {
      return {
        maxHeight: this.$vuetify.breakpoint.xsOnly
          ? this.global.innerHeight - 470
          : 300
      };
    },
    schoolName() {
      return this.getSchoolName();
    },
    showBox() {
      return !!this.search && this.search.length > 0;
    },
    classes() {
      return this.getClasses();
    },
    //edge hide placehloder fix
    placeholderVisible() {
      return this.getSelectedClasses.length < 1;
    },
    selectedClasses: {
      get() {
        return this.getSelectedClasses;
      },
      set(val) {
        let arrValidData = [];
        if (val.length > 0) {
          arrValidData = val.filter(singleClass => {
            if (singleClass.text) {
              return singleClass.text.length > 3;
            } else {
              return singleClass.length > 3;
            }
          });
        }
        this.updateSelectedClasses(arrValidData);
      }
    }
  },
  filters: {
    boldText(value, search) {
      if (!value) return "";
      if (!search) return value;
      let match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
      if (match) {
        let startIndex = value.toLowerCase().indexOf(search.toLowerCase());
        let endIndex = search.length;
        let word = value.substr(startIndex, endIndex);
        return value.replace(word, "<b>" + word + "</b>");
      } else {
        return value;
      }
    }
  },
  methods: {
    ...mapActions(["updateClasses", "updateSelectedClasses", "assignClasses", "pushClassToSelectedClasses"]),
    ...mapGetters(["getSchoolName", "getClasses"]),

    lastStep() {
      this.fnMethods.changeStep(this.enumSteps.set_school);
    },
    nextStep() {
      //TODO add action update the server instead of 'updateSelectedClasses'
      this.assignClasses().then(() => {
        this.fnMethods.changeStep(this.enumSteps.done);
      });
    },
    addClass(className) {
      this.pushClassToSelectedClasses(className);
      setTimeout(()=>{
        let container = document.querySelector('.v-select__selections');
        let inputElm = container.querySelector('input');
        inputElm.value = "";
        inputElm.focus();
      }, 200)
    },
    itemInList(item) {
      if (typeof item !== "object") {
        return false;
      } else {
        return true;
      }
    }
  }
};
</script>

<style lang="less" scoped>

.chip-style {
  background-color: rgba(68, 82, 252, 0.09);
  &.dark-chip {
    background-color: rgba(68, 82, 252, 0.27);
  }
  &.selected {
    -webkit-box-shadow: 0px 2px 4px 0 rgba(0, 0, 0, 0.2);
    -moz-box-shadow: 0px 2px 4px 0 rgba(0, 0, 0, 0.2);
    box-shadow: 0px 2px 4px 0 rgba(0, 0, 0, 0.2);
  }
}

.chip-button {
  cursor: pointer;
  max-width: 150px;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}

.sbf-close {
  font-size: 8px !important;
  margin-bottom: 3px;
  margin-left: 8px;
}

.subheading {
  font-size: 16px;
  color: rgba(0, 0, 0, 0.38);
  &.dark {
    font-size: 13px !important;
    color: rgba(0, 0, 0, 0.54);
  }
}
</style>
