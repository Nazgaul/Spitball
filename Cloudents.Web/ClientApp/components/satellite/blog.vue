<template>
    <!--<general-page>-->
    <div class="blog-wrap">
        <v-layout row v-if="uni" class="uni" justify-center>
            <img :src="uni.image" class="mr-3 elevation-2" />
            <h5>{{uni.name}}</h5>
        </v-layout>
        <iframe :src="src"></iframe>
    </div>
    <!--</general-page>-->
</template>


<script>
    import generalPage from "./general.vue";
    import help from "../../services/satelliteService";
    export default {
        components: { generalPage },
        data() {
            return {
                uni: null,
                src: "https://spitballco.wordpress.com/"
            }
        },
        created() {
            let append = this.$route.query.path || "";
            append.replace("https://spitballco.wordpress.com/", "")
            append.replace("http://spitballco.wordpress.com/", "")
            this.src += append;
            if (this.$route.query.uni) {
                help.getBlog(this.$route.query.uni).then(val => {
                    this.uni = val;
                });
            }

        }
    }
</script>
<style lang="less">
    .blog-wrap {
        background:#f0f0f0;
        padding-top:22px;
       


     iframe {
        width: 100%;
        height: 100vh;
        border: 0;
    }

    

    img {
        width: 56px;
        height: 56px;
    }

    h5 {
        font-size: 24px;
        letter-spacing: -0.4px;
        color: #4a4a4a;
        line-height:56px;
        vertical-align:middle;
        font-weight:normal;
    }

    .uni {
        margin: 0 auto 5px;
    }
    
    }
</style>
