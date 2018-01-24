<template>
    <button type="button" class="suggest" @click="$ga.event('Suggest_Window',$route.path.slice(1),name)">
        <v-container>
            <v-layout class="wrapper hidden-md-and-down" row justify-space-between >
                    <div class="logo">Spitball</div>
                    <div class="action">Show me</div>
            </v-layout>
            
            <v-layout class="icon hidden-lg-and-up">
                <logo></logo>
            </v-layout>
            <div class="text">
                {{suggestList[name]}}
                <!--<img src="./img/smily.png"
                     srcset="./img/smily@2x.png 2x,
                                ./img/smily@3x.png 3x"
                    >-->
            </div>
        </v-container>
    </button>
</template>
<script>
    import logo from "./svg/sb-logo.svg";
    const suggestList={
        job:"Need some cash? Find the best jobs & internships for students at your school.",
        tutor:"Still need help? Connect with a tutor and make sure you ace your class.",
    note:"Searching for study documents for your classes? Look no further.",
    book:"Buying or selling a textbook? We'll make sure you get the best price!",
    ask:"Have a question you need answered? Try us.",
        flashcard:"We found some flashcards that may help you study!"};
    export default {
        components: { logo },
        data(){
            return{suggestList}
            },
        props: {
            name: { type: String }
        }
    }
</script>
<style src="./suggestCard.less" lang="less" scoped></style>