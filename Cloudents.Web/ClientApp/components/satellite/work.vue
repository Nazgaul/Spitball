<template>
    <general-page>
        <div class="work">
            <v-layout class="strip" v-for="(data,key,index) in pageData"  row align-center justify-space-between :reverse="$vuetify && $vuetify.breakpoint.smAndUp && index%2" :key="index">
                <v-flex class="text">
                    <h2>{{data.title}}</h2>
                    <p>{{data.text}}</p>
                </v-flex>
                <div class="gap"></div>
                <img :src="require(`./img/${data.image}`)" />
            </v-layout>
        </div>
    </general-page>
</template>


<script>
    import generalPage from "./general.vue";
    const pageData = {
        whatIsSpitball: {
            image: "how-spitball-works-1.png",
            title: "How Spitball Works",
            text: "Our Homework Help offers students a quick and simple way to Ask and Answer tricky homework questions.  Spitball incentivizes knowledgeable students to support one another for profit, bringing students from all over the world together to help each other succeed in school. Ask Your Question OR answer someone else’s, a win-win!"
        },
        Technology: {
            image: "how-spitball-works-2.png",
            title: "Our Technology",
            text: `Our platform combines artificial intelligence & machine learning with blockchain technology. This means we are able to bring you customized, relevant content while eliminating the “middle man” (aka, outdated banking systems and other controlling websites), enabling students to directly exchange knowledge, goods and services. By doing this, we can greatly reduce transaction fees and put the power back in the hands of students! 
                   Spitball’s technology uses “smart contracts” (a piece of code programmed to execute automatically) to keep track of your knowledge and reward you for asking and answering questions; in a safe, secure & extremely efficient way. It doesn’t get any better than this! We promise.`

        },
        Personalization: {
            image: "how-spitball-works-3.png",
            title: "Cashing Out",
            text: `Ah, the good stuff. So you wanna cash out? No problem! Just go to your wallet (make sure you have enough redeemable SBL), click on Cash Out and choose your reward. Your Amazon Coupon will be sent to your email within 24 hours.\n
             Easy Peasy! \n
             Tip: We encourage students to save some SBL Tokens in order to ask tough homework questions in the future.`
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