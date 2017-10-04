<template>
    <div>
        <a target="_blank" :href="item.url" v-for="item in items">
            <v-card class="wrapper">
                <div class="container-box">
                    <div class="box-main box-user">
                        <div class="img">
                            <span class="up">
                                <img :src="item.image" alt="">
                            </span>
                        </div>
                        <div class="box-wrapper">
                            <v-layout row>
                                <v-flex xs8><div class="title">{{item.name}}</div></v-flex>
                                <v-flex xs4 v-if="item.fee"><div class="price text-xs-right">${{item.fee}} / hour</div></v-flex>
                            </v-layout>
                            <div class="sub-title">Engineering student and math tutor!</div>
                            <p>I am a current materials science engineering student at the University of Michigan…</p>
                            <div class="author">{{item.city}}, {{item.state}}</div>
                            <div class="state" v-if="item.online">Offers online lessons</div>
                        </div>
                    </div>
                </div>
            </v-card>
        </a>
    </div>
</template>
<script>
    export default {
        computed: {
            items: function () { return this.$store.getters.items }
        }
    }
</script>