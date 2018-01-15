<template>
    <v-toolbar app fixed :height="$vuetify.breakpoint.mdAndUp ? 140 : 182" class="header elevation-0">
        <v-layout column class="header-elements">
            <v-layout column class="header-elements">
                <main-header class="elevation-1"></main-header>
            </v-layout>
            <slot name="header">
                <v-toolbar class="item-toolbar elevation-0" height="80">
                    <v-layout class="toolbar-content" column>
                        <v-flex>
                            <v-layout row align-center justify-space-between>
                                <div class="item-name">{{item.name}}</div>
                                <!--<item-actions></item-actions>-->
                            </v-layout>
                        </v-flex>
                        <v-flex class="item-meta mt-2">
                            <v-layout row align-center justify-space-between>
                                <div class="author">{{item.author}} woop</div>
                                <div class="date">{{item.date | mediumDate}}</div>
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
    import {mapGetters} from 'vuex'
    export default {
        components: { mainHeader },
        computed:{...mapGetters({'item':'itemDetails'})},
        filters: {
            mediumDate: function (value) {
                if (!value) return '';
                let date = new Date(value);
                return date.toLocaleString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
            }
        }
    }
</script>