<template>
    <v-toolbar app fixed :height="140" class="header elevation-0">
        <v-layout column class="header-elements">
            <v-layout column class="header-elements">
                <main-header class="elevation-1"></main-header>
            </v-layout>
            <slot name="header">
                <v-toolbar class="item-toolbar elevation-0" height="80">
                    <v-layout class="toolbar-content" column>
                        <v-flex>
                            <v-layout row align-center justify-space-between>
                                <div class="item-name">item.name}}</div>
                                <item-actions></item-actions>
                            </v-layout>
                        </v-flex>
                        <v-flex class="item-meta mt-2">
                            <v-layout row align-center justify-space-between>
                                <div class="author">item.author}} woop</div>
                                <div class="date">item.date | mediumDate}}</div>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                </v-toolbar>
            </slot>

        </v-layout>
    </v-toolbar>
</template>

<script>
    import mainHeader from '../helpers/header.vue'
    export default {
        components: { mainHeader }
    }
</script>