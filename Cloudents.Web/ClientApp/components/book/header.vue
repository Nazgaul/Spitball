<template>
    <div class="book-header">
        <dialog-toolbar :app="true" height="48" v-if="$vuetify.breakpoint.xsOnly" :toolbarTitle="toolbarTitleText" v-language:toolbarTitle>
            <v-tabs class="mx-2" value="buy" grow slot="extension">
                    <v-tab v-for="tab in sortOptions" :key="tab.id" :id="tab" @click="$_changeTab(tab)"
                                 class="mr-2 vertical spitball-text-book">
                        {{tab}}
                    </v-tab>
                    <v-tabs-slider color="color-book"></v-tabs-slider>
            </v-tabs>
        </dialog-toolbar>
        
        <regular-header v-else
                        currentSelection="book"
                        submitRoute="/book"></regular-header>
    </div>
</template>

<script>
    import regularHeader from "../header/header.vue"
    import TabMixin from "./bookDetailsSort"
    import DialogToolbar from '../dialog-toolbar/DialogToolbar.vue'
    import {LanguageService} from '../../services/language/languageService'
    export default {
        mixins: [TabMixin],
        components: { regularHeader, DialogToolbar },
        data(){
            return{
                toolbarTitleText: LanguageService.getValueByKey("book_header_textbook")
            }
        }
    }
</script>
<style src="./header.less" lang="less"></style>