<template>
    <div class="tutor-search-input">
        <div class="search-b-wrapper">
            <v-text-field class="search-b" type="text" solo
                          :class="{'white-background': showSuggestions}"
                          @keyup.enter="search()" autocomplete="off" 
                          @keydown.down="arrowNavigation(1)"
                          @keydown.up="arrowNavigation(-1)"
                          id="tutorSearchInput"
                          clearable
                          clear-icon="sbf-close"
                          v-model="msg" 
                          @input="changeMsg"
                          :placeholder="$t('tutorListLanding_search_placeholder')"
                          prepend-icon="sbf-search">
            </v-text-field>
                <v-list class="search-menu" v-show="showSuggestions" v-click-outside="closeSuggestions">
                    <template v-for="(item, index) in suggests">
                        <v-list-item class="suggestion" @click="selectors(item)" :key="index" :class="{'list__tile--highlighted': index === focusedIndex}">
                            <v-list-item-content>
                                <v-list-item-title>{{item.text}}</v-list-item-title>
                            </v-list-item-content>
                        </v-list-item>
                    </template>
                </v-list>
        </div>
        <slot name="searchBtn" :search="search"></slot>
    </div>
</template>

<script>
import debounce from "lodash/debounce";
import courseService from "../../../../services/courseService";
import analyticsService from '../../../../services/analytics.service';
import { Learning } from '../../../../routes/routeNames';
export default {
    name: "tutor-search-input",
    props: {
        placeholder:{
            type:String,
            required:false
        }
    },
    data: () => ({
        autoSuggestList: [],
        showSuggestions: false,
        originalMsg: '',
        msg:"",
        suggests: [],
        term: '',
        focusedIndex: -1,
    }),
    computed: {
    },
    watch: {
        '$route'() {
            this.$forceUpdate();
        }
    },
    methods: {
        search(text) {
            if(!!text){
                this.msg = text;
            }else if(!this.msg) {
                this.msg = '';
            }
            if(!!this.msg){
                analyticsService.sb_unitedEvent("Tutor_Engagement", "Search", this.msg);
            }

            //this.$router.push({ path: `/tutor-list/${encodeURIComponent(this.msg)}` }).catch(() => {});
            this.$router.push({ name: Learning, params: {course: this.msg || undefined} }).catch(() => {});
            
            this.closeSuggestions();
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
            });
        },
        openSuggestions() {            
            this.showSuggestions = true;
        },
        changeMsg: debounce(function (val) {
            if(!!val && val.length > 0) {
                this.getSuggestionList(val);
            } else {
                this.closeSuggestions();
            }
        }, 250),
        closeSuggestions() {
            if(this.showSuggestions) {
                this.focusedIndex = -1;
                this.originalMsg = "";
                this.showSuggestions = false;
            }
        },
        getSuggestionList(term){
            let self = this
            courseService.getCourse({term}).then(data=>{
              self.suggests = data;
            }).finally(()=>{
              if(self.suggests.length) {
                self.openSuggestions();
                return
              }
              self.showSuggestions = false
            });

        },
        selectors(item){
            this.search(item.text);
        },
        focusOnSelectedElm(searchMenu, direction){
            const ELM_HEIGHT_WITH_MARGIN = 51;
            const ELM_HEIGHT_WITHOUT_MARGIN = 36;
            let containerHeight = searchMenu.parentElement.offsetHeight;
            let currentHighlightedElm = searchMenu.offsetTop + ELM_HEIGHT_WITH_MARGIN;
            if(direction === 1) {
                if((currentHighlightedElm + ELM_HEIGHT_WITHOUT_MARGIN) > containerHeight){
                    let stepsToScroll = Math.floor(currentHighlightedElm / ELM_HEIGHT_WITH_MARGIN);
                    let newScrollVal = stepsToScroll * ELM_HEIGHT_WITH_MARGIN;
                    searchMenu.parentElement.scrollTop = newScrollVal;
                } else {
                    searchMenu.parentElement.scrollTop = 0;
                }
            } else {
                if(searchMenu.parentElement.scrollTop > 0 && Math.floor(searchMenu.parentElement.scrollTop/ELM_HEIGHT_WITH_MARGIN) > 1) {
                    currentHighlightedElm = currentHighlightedElm - ELM_HEIGHT_WITH_MARGIN;
                    let stepsToScroll = Math.floor(currentHighlightedElm / ELM_HEIGHT_WITH_MARGIN);
                    let newScrollVal = stepsToScroll * ELM_HEIGHT_WITH_MARGIN;
                    searchMenu.parentElement.scrollTop = newScrollVal - ELM_HEIGHT_WITH_MARGIN;
                } else if((currentHighlightedElm - ELM_HEIGHT_WITHOUT_MARGIN) < containerHeight) {
                    searchMenu.parentElement.scrollTop = 0;
                } else {
                    searchMenu.parentElement.scrollTop = currentHighlightedElm * ELM_HEIGHT_WITH_MARGIN;
                }
            }
        },
        arrowNavigation(direction) {
            // When to save user's typed text

            let searchMenu = document.querySelector('.list__tile--highlighted');

            // saving user input text
            if (this.focusedIndex === -1) {
                this.originalMsg = this.msg;
            }

            // Handling arrows:

            if(direction > 0) {
                if(this.suggests.length -1 === this.focusedIndex) {
                    return;
                }
                this.focusedIndex = this.focusedIndex + direction;
            }  else { 
                if(this.focusedIndex < 0) {
                    return;
                } 
                this.focusedIndex = this.focusedIndex + direction;
            }
        
            if(this.focusedIndex > -1 && this.focusedIndex < this.suggests.length) {
                this.msg = this.suggests[this.focusedIndex].text;
            }

            if(searchMenu){
                this.focusOnSelectedElm(searchMenu, direction);
            } else {
                return;
            }
            
            // Out of bounds - set index to be -1:
            if (this.focusedIndex === this.suggests.length || this.focusedIndex < 0) {
                this.msg = this.originalMsg;
            }
        }
    },
    created() {
        this.msg = !!this.$route.params && !!this.$route.params.course ? this.$route.params.course : '';
    }
};
</script>
<style lang="less">
@import "../../../../styles/mixin.less";
.tutor-search-input {
  width: 100%;
  min-width: auto;
  border-radius: 4px;
  background-color: @color-white;
  // @media (max-width: @screen-xs) {
  //   margin-left: 0;
  // }
  .search-b{
    background-color: @color-white;
    border-radius: 4px;
    &.white-background {
      background-color: @color-white;
      box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.25);
    }
    .v-text-field__details{
      padding: 0;
    }
  }

  .search-back-arrow {
    line-height: 32px;
    i {
      transform: rotate(180deg);
      line-height: 32px;
      margin-right: 20px;
      font-size: 14px;
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
   // flex-grow: 1;
    .v-input__prepend-outer {
      margin-top: 16px;
      margin-left: 12px;
    }
    .v-input__control {
      height:56px;
      .v-input__slot {
        height:100%;
        box-shadow: none !important;
        background: none;
        margin-bottom: 0!important;
        input {
          padding-right: 15px;
        }
        //clear icon style
        .v-input__append-inner {
          margin-top:10px;
          .v-icon {
            &.sbf-close {
              font-size: 10px;
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

  .search-b, .search-menu {
    z-index: 3;
  }

  .menu-toggler {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
  }

  .search-menu {
    position: absolute;
    top: 52px;
    left: 0;
    right: 0;
    background: @color-white !important;
    max-height: 440px;
    margin-top: 1px;
    box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.15) !important;
    overflow: auto;
    padding: 0;
    border-top: none;
    @media (max-width: @screen-xs) {
      max-height: 290px;
      // margin-top: 10px;
    }

    .suggestion {
      margin: 15px 0;
      &.list__tile--highlighted {
        background: rgba(48, 46, 181,.1);
      }
    }

    .list {
      padding: 6px 0;
    }

    .subheader {
      color: @color-grey;
      letter-spacing: -0.3px;
      font-size: 18px;
      line-height: 2.22;
      height: 20px;
      padding-left: 20px;
      padding-top: 5px;
      margin-bottom: 15px;

      @media (max-width: @screen-xs) {
        font-size: 14px;
        height: 24px;
        margin-bottom: 8px;
      }
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
    /*.list__tile__title {
            line-height: normal;
        }*/
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
        padding-right: 16px;
        min-width: 40px;

        i, svg {
          margin: 0 auto;
        }

        @media (max-width: @screen-xs) {
          min-width: 24px;

          .v-icon {
            font-size: 16px;
          }
        }
      }
    }
  }

  button {
    //font-family: @font-bold;
    margin: 0 0 0 16px;
    padding: 0 24px;
    width: 106px;
    border-radius: 4px;
    z-index: 1;

    i.v-icon {
      color: @color-white;
    }

    @media (max-width: @screen-xs) {
      margin: 0;
      border-top-left-radius: 0;
      border-bottom-left-radius: 0;
      width: 40px;
      padding: 0;
    }
  }
}

// .uni-field .search-menu {
//   .v-list__tile {
//     height: 56px;
//     .suggestion-image {
//       width: 40px;
//       height: 40px;
//     }
//   }
//   .v-list__tile__title {
//     color: black;

//     .highlight {
//       color: #302eb5;
//     }
//   }
// }
</style>
