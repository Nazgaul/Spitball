﻿import askHeader from '../navbar/images/ask.svg'
import flashcardHeader from '../navbar/images/flashcard.svg'
import jobHeader from '../navbar/images/job.svg'
import bookHeader from '../navbar/images/book.svg'
import noteHeader from '../navbar/images/document.svg'
import tutorHeader from '../navbar/images/tutor.svg'
import courseHeader from '../navbar/images/courses.svg'
import settingHeader from '../navbar/images/setting.svg'
import foodHeader from '../navbar/images/food.svg'
import searchTypes from './../helpers/radioList.vue'
import searchIcon from './Images/search-icon.svg';
import hamburger from './Images/hamburger.svg';
import { mapActions,mapGetters} from 'vuex'
//verticalsPlaceholders as placeholders,
import {  names } from '../data'
import logo from '../../../wwwroot/Images/logo-spitball.svg';
import notification from "./images/notification-icon.svg";

export default {
    components: {
        'search-type': searchTypes,
        askHeader,
        bookHeader,
        noteHeader,
        flashcardHeader,
        settingHeader,
        jobHeader,
        courseHeader,
        foodHeader,
        tutorHeader,
        logo,
        "resultHeader":askHeader,
        "notification-icon" : notification,
        "search-icon": searchIcon,
        "bookDetailsHeader": bookHeader,
        hamburger
    },
    data() {
        return {
           // placeholders: placeholders,
            names: names,
            currentName:'',
            qFilter: this.$route.query.q,
            snackbar:true
        };
    },

    computed: {
        ...mapGetters(['luisTerm']),
        name: function () {
            let currentPage = this.$route.meta.pageName ? this.$route.meta.pageName : this.$route.path.split('/')[1];
            if (this.currentName !== currentPage) {
                this.currentName = currentPage;
                    if (this.$route.query.q) {
                        this.qFilter = this.$route.query.q;
                        this.$emit('update:userText', this.qFilter);
                    }
                }
                return this.currentName;
        }
    },
    watch:{
      '$route':function(val){
          this.qFilter=val.query.q;
      }
    },
    props:{value:{type:Boolean}},
    methods: {
        ...mapActions(['updateSearchText']),
        submit: function () {
            this.updateSearchText(this.qFilter).then((response) => {
                let result=response.result;
                this.$route.meta[result.includes('food')?'foodTerm':result.includes('job')?'jobTerm':'term']={
                    term: this.qFilter,
                    luisTerm: response.term
                };
                this.$router.push({ path: response.result, query: { q: this.qFilter } });
                this.$emit('update:userText', this.qFilter);
            });
            this.$emit('update:overlay', false);
        },
        menuToggle: function() {
            this.$emit('input',!this.value);
        }
    }
}
