<template>
    <div class="menuListStudent">
        <v-list-item v-for="link in satelliteLinksStudent" :to="{ name: link.url }" :key="link.title" sel="menu_row" >
            <v-list-item-action>
                <v-icon class="userMenu_icons">{{link.icon}}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                <v-list-item-title><span class="userMenu_titles">{{link.title}}</span></v-list-item-title>
            </v-list-item-content>
        </v-list-item>
    </div>
</template>

<script>
import satelliteService from '../../../../services/satelliteService';

export default {
    props: {
        satelliteLinksStudent: {}
    },
    data() {
        return {
            satelliteService
        }
    }
}
</script>