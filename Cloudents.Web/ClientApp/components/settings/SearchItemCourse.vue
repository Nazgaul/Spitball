<template>
    <v-list-tile :key="item.id">
        <v-list-tile-content>
            <v-list-tile-title v-text="item.code"></v-list-tile-title>
            <v-list-tile-sub-title v-text="item.name"></v-list-tile-sub-title>
        </v-list-tile-content>
        <v-list-tile-action>
            <v-list-tile-action-text><input type="checkbox" :value="item.id" :id="item.id" :checked="isSelected"/></v-list-tile-action-text>
        </v-list-tile-action>
    </v-list-tile>
    </template><script>
        export default {
            model: {
                prop: 'value',
                event: 'selected'
            },
            props: { item: { type: Object, required: true }, value: {} },

            computed: {
                isSelected: function () { return this.value.includes(this.item.id)}
            }
    }
    </script>