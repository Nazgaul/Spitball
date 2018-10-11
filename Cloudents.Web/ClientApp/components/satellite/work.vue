<template>
    <general-page>
        <div class="work">
            <v-layout class="strip" v-for="(data, key,index) in pageData"  row align-center justify-space-between :reverse="$vuetify && $vuetify.breakpoint.smAndUp && index%2" :key="index">
                <v-flex class="text">
                    <h2>{{data.title}}</h2>
                    <p>{{data.text}}</p>
                </v-flex>
                <div class="gap"></div>
                <img :src="require(`./img/${data.image}`)"/>
            </v-layout>
        </div>
    </general-page>
</template>


<script>
    import generalPage from "./general.vue";
    import { LanguageService } from "../../services/language/languageService";

    const pageData = {
        whatIsSpitball: {
            image: "how-spitball-works-1.png",
            title: LanguageService.getValueByKey("work_whatIsSpitabll_title"),
            text: LanguageService.getValueByKey("work_whatIsSpitabll_text")
        },
        Technology: {
            image: "how-spitball-works-2.png",
            title: LanguageService.getValueByKey("work_technology_title"),
            text: LanguageService.getValueByKey("work_technology_text"),

        },
        Personalization: {
            image: "how-spitball-works-3.png",
            title: LanguageService.getValueByKey("work_personalization_title"),
            text:  LanguageService.getValueByKey("work_personalization_text"),
        },
    }
    export default {
        components: { generalPage },
        data() {
            return { pageData }
        }
    }
</script>
<style src="./work.less" lang="less"></style>