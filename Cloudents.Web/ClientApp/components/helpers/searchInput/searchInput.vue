<template>
    <div class="search-input">
        <div class="search-b-wrapper">
            <v-text-field 
                class="search-b" type="search" solo
                :class="{'white-background': showSuggestions}"
                @keyup.enter="search()" autocomplete="off"
                name="q"
                id="transcript"
                v-model="msg"
                :placeholder="placeholder"
                prepend-icon="sbf-search"
                :clear-icon="'sbf-close'"
                :hide-on-scroll="isHome ? hideOnScroll : false">
            </v-text-field>
            <div class="menu-toggler" v-show="showSuggestions" @click="closeSuggestions"></div>
        </div>
        <slot name="searchBtn" :search="search"></slot>
    </div>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex';
import { LanguageService } from "../../../services/language/languageService";
import analyticsService from '../../../services/analytics.service';

export default {
    name: "search-input",
    props: {
        hideOnScroll: {
            type: Boolean,
            default: false
        },
        placeholder: {
            type: String
        },
        userText: {
            String
        },
        submitRoute: {
            String,
            default: '/feed'
        }
    },
    data: () => ({
        // autoSuggestList: [],
        isFirst: true,
        showSuggestions: false,
        focusedIndex: -1,
        originalMsg: '',
        suggestOptions: [
            {
                name: LanguageService.getValueByKey("searchInput_class_search"),
                icon: "classIcon",
                analyticName: 'Class',
                id: 1
            },
            {
                name: LanguageService.getValueByKey("searchInput_university_search"),
                icon: "universityIcon",
                analyticName: 'University',
                id: 2
            },
            {
                name: LanguageService.getValueByKey("searchInput_spitball_search"),
                icon: "spitballIcon",
                analyticName: 'Spitball',
                id: 3
            }
        ]
    }),
    computed: {
        suggestList() {
            let dynamicSuggest;
            if(this.courseSelected()) {
                dynamicSuggest = [].concat(this.suggestOptions);
            } else {
                dynamicSuggest = this.suggestOptions.filter(searchOption => {
                    return searchOption.id !== 1;
                });
            }
            return dynamicSuggest;
        },

        isHome() {
            return this.$route.name === 'home';
        },
    },
    watch: {
        userText(val) {
            this.msg = val;
            this.isFirst = true;
        },
        focusedIndex(val) {
            if(val < 0) {
                this.msg = this.originalMsg;
            } else {
                this.msg = this.suggestList[this.focusedIndex].text;
            }
        },
        '$route'() {
            this.$forceUpdate();
        }
    },
    methods: {
        ...mapGetters(['getUniversityPopStorage', 'accountUser', 'getSchoolName']),
        ...mapActions(['changeSelectPopUpUniState', 'setUniversityPopStorage_session']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        
        goBackFromSearch() {
            this.msg = "";
            this.search();
        },
        courseSelected() {
            return !!this.$route.query ? !!this.$route.query.Course : false;
        },
        isRouteDuplication(newRoute, query){
            return this.$router.currentRoute.path === newRoute && JSON.stringify(query) === JSON.stringify(this.$router.currentRoute.query);
        },
        search(item) {
            let id = item? item.id : undefined;
            //prevent search empty string when no term is in search ATM
            if(!this.$route.query.term) {
                if(!this.msg) return;
            }
            this.UPDATE_SEARCH_LOADING(true);
            let query = this.prepareQuery(id);
            let sameRoute = this.isRouteDuplication(this.submitRoute, query);
            this.$router.push({ path: this.submitRoute, query });

            if(this.msg) {
                let suggestOptions;
                if(id) {
                    suggestOptions = item.analyticName;
                } else {
                    if(this.courseSelected()){
                        suggestOptions = 'Class';
                    }else{
                        suggestOptions = 'Spitball';
                    }
                }
                analyticsService.sb_unitedEvent('global_search', suggestOptions, this.msg);
            }
            this.closeSuggestions();
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
                if(sameRoute){
                    setTimeout(()=>{
                        this.UPDATE_SEARCH_LOADING(false);
                    }, 300);
                }
            });
        },
        prepareQuery(typeId) {
            let query = { term: this.msg };
            if(!this.msg) {
                delete query.term;
            }
            if(typeId === 1) {
                query.Course = this.$route.query.Course;
                query.uni = this.getSchoolName();                
            } else if(typeId === 2) {
                query.uni = this.getSchoolName();
            } else if(typeId === 3) {
                //query should incude only term
            } else {
                if(!!this.$route.query.Course) {
                    query.Course = this.$route.query.Course;
                }
            }
            return query;
        },
        openUniPop() {
            console.log("uni select pop");
            this.changeSelectPopUpUniState(true);

        },
        showUniPop() {
            let user = this.accountUser();
            if(!!user && !user.universityExists) {
                let uniStoragePop = this.getUniversityPopStorage();
                if(uniStoragePop.local < 3 && !uniStoragePop.session) {
                    this.openUniPop();
                    this.setUniversityPopStorage_session();
                    return true;
                }
                return false;
            } else {
                return false;
            }
        },
        openSuggestions() {
            //if user with no university pop it, up to 3 times in seperated seassons
            if(this.showUniPop()) {
                return;
            }
            this.showSuggestions = true;
        },
        closeSuggestions() {
            this.focusedIndex = -1;
            if(this.showSuggestions) {
                this.showSuggestions = false;
            }
        }
    },
    created() {
        if(!this.isHome) {
            this.msg = this.userText || '';
        }
    }

};
</script>

<style lang="less">

@import "../../../styles/mixin.less";
.search-input {
  display: flex;
  width: 100%;
  max-width: @cellWidth;
  min-width: auto;
  border-radius: 4px;
  margin-left: 48px;
  background-color: @systemBackgroundColor;
  @media (max-width: @screen-xs) {
    margin-left: 0;
    border: solid 1px #d1d4d5; 
  }
  .search-b{
    background-color: @systemBackgroundColor;
    &.white-background {
      background-color: @color-white;
      box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.25);
    }
  }

  .search-back-arrow {
    line-height: 32px;
    i {
      transform: rotate(180deg);
      line-height: 32px;
      margin-right: 20px;
      font-size: 14px !important;
      margin-left: 14px;
      cursor: pointer;
    }
  }

  .search-menu .type-AutoComplete .v-list__tile__title {
    color: black;

    .highlight {
      color: #302eb5;
    }

    .suggestion.focused .v-list__tile {
      background: rgba(0, 0, 0, .12);
    }
  }

  .search-b-wrapper {
    position: relative;
    flex-grow: 1;
    .v-input__prepend-outer {
      margin-top: 8px;
      margin-left: 12px;
    }
    .v-input__control {
      min-height: 40px;
      .v-input__slot {
        box-shadow: none !important;
        background: none;
        padding: 0;
        margin-bottom: 0!important;
        input {
          padding-right: 15px;
        }
        .v-input__append-inner {
          .v-icon {
            &.sbf-close {
              font-size: 10px !important;
              padding: 0 8px 8px 0;
            }

          }
        }
      }
    }
    input[type="search"] {
      .clear-field-btn;
      font-size: 14px;
    }

  }

  .search-b {
    z-index: 3;
  }

  .menu-toggler {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    height: 100vh;
  }
    .v-list__tile__title {
      color: #302eb5;
      height: 37px;
      font-size: 18px;
      line-height: 2;
      letter-spacing: -0.3px;

      @media (max-width: @screen-xs) {
        font-size: 14px;
        letter-spacing: -0.2px;
      }
    }
    .v-list__tile {
      height: 36px;

      @media (max-width: @screen-xs) {
        height: 27px;
      }
    }

    .v-list__tile--link {
      padding-left: 14px;

      @media (max-width: @screen-xs) {
        padding-left: 8px;
      }

      .v-list__tile__action {
        min-width: 0;
        padding-right: 16px;
        min-width: 40px;

        i, svg {
          margin: 0 auto;
        }

        @media (max-width: @screen-xs) {
          min-width: 24px;

          .v-icon {
            font-size: 16px !important;
          }
        }
      }
    }
  }

</style>
