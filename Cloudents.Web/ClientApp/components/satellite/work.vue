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
            title: "What is Spitball?",
            text: "Simply put, Spitball is the global marketplace for students. Spitball allows students to ask and answer questions, share notes, find tutors, buy & sell textbooks and more - making it the ideal one-stop-shop for students. We bring the best that the internet has to offer students, all together in one place."
        },
        Technology: {
            image: "how-spitball-works-2.png",
            title: "Our Technology",
            text: "Our platform combines artificial intelligence & machine learning with blockchain technology, bringing students the most relevant content and enabling them to exchange peer-to-peer (P2P) safely & securely. We source directly from students and leading educational websites, so you can forget about the time-consuming and frustrating search process through the tsunami of online content."
        },
        Personalization: {
            image: "how-spitball-works-3.png",
            title: "Personalization",
            text: "Spitball offers personalized results based on your school, classes, and preferences. Our AI engine combined with your specific user preferences provides curated, relevant results for you. Missed class and need some notes? Spitball it. Need a tutor? Spitball it. Buying textbooks? You get the idea..."
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