<template>
    <v-list-tile :key="item.id">
        <v-list-tile-content>
            <v-list-tile-title v-text="item.code"></v-list-tile-title>
            <v-list-tile-sub-title v-text="item.name"></v-list-tile-sub-title>
        </v-list-tile-content>
    </v-list-tile>
    </template><script>
    export default {
            props: { item: {type:Object,required:true}}
    }
    </script>