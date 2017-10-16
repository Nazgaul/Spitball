<template>
    <a target="_blank" :href="item.link">
        <v-layout wrap row>
            <v-flex xs4>
                <img v-if="item.image" :src="item.image" />
                <span v-else>{{item.name}}</span>
            </v-flex>
            <v-flex xs4>
                condition:{{item.condition}}
            </v-flex>        <v-flex xs4>price:{{item.price}}</v-flex>
        </v-layout>
    </a>
</template>
<script>
    export default {
        props: { item: {type:Object}}
    }
</script>