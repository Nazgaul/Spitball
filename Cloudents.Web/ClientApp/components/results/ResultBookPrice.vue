<template>
    <a target="_blank" :href="item.link">
        <v-layout wrap row>
            <v-flex xs4>
                <img v-if="item.image" :src="item.image" />
                <span v-else>{{item.name}}</span>
            </v-flex>
            <v-flex xs4 class="black--text">
                {{item.condition}}
            </v-flex>
            <v-flex xs4 class="price">
                ${{item.price}}
            </v-flex>
        </v-layout>
    </a>
</template>
<script>
    export default {
        props: { item: { type: Object } }
    }
</script>
<style scoped>
    /*.price {
        width: 88px;
        height: 24px;
        border-radius: 2px;
        background-color: #ebbc18;
        text-align: center;
    }*/
</style>