<template>
    <div>
        <v-list-item
            v-for="link in satelliteLinksTeacher"
            :href="link.url || ''"
            @click="link.action ? link.action() :  ''"
            :key="link.title"
            sel="menu_row"
            link
            target="_blank"
        >
            <v-list-item-action>
                <v-icon class="userMenu_icons">{{link.icon}}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                {{link.title}}
            </v-list-item-content>
        </v-list-item>
    </div>
</template>

<script>
export default {
    props: {
        satelliteLinksTeacher: {
            required: true
        },
        satelliteLinksStudent: {
            required: false // prevent from dom attr to show [object object]
        }
    }
}
</script>