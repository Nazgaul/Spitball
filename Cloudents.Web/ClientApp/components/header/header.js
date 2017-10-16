﻿import askHeader from '../general/images/ask.svg'
import flashcardHeader from '../general/images/flashcard.svg'
import jobHeader from '../general/images/job.svg'
import bookHeader from '../general/images/book.svg'
import noteHeader from '../general/images/document.svg'
import tutorHeader from '../general/images/tutor.svg'
import foodHeader from '../general/images/food.svg'
import searchTypes from './../helpers/radioList.vue'
import { mapGetters, mapActions } from 'vuex'
import { verticalsPlaceholders as placeholders, names } from '../data'
import logo from '../../../wwwroot/Images/logo-spitball.svg';

export default {
    components: {
        'search-type': searchTypes,
        askHeader,
        bookHeader,
        noteHeader,
        flashcardHeader,
        jobHeader,
        foodHeader,
        tutorHeader,
        logo
    },
    data() {
        return {
            placeholders: placeholders,
            showOption: false,
            names: names,
            qFilter: this.userText,
            isOptions: false
        }
    }, computed: {
        ...mapGetters(['userText'])
    },

    props: {
        openOptions: { type: Function }
    },

    mounted: function () {
        this.qFilter = this.userText;
    },

    methods: {
        ...mapActions(['updateSearchText']),
        changeType: function (val) {
            this.updateSearchText();
            this.qFilter = "";
            this.$router.push({ name: val });
            this.updateSearchText();
        },
        submit: function () {
            this.isOptions = false;
            this.updateSearchText(this.qFilter);
            this.$emit('update:overlay', false);
        },
        focus: function () {
            this.isOptions = true;
            this.$emit('update:overlay', true);
        }
    }
}
