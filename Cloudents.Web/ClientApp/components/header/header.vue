<template>
        <main-header ref="mainHeader" :userText="userText" 
                     :submitRoute="submitRoute" :currentSelection="currentSelection"
                     :toolbarHeight="$vuetify.breakpoint.mdAndUp ? 120 : 152">
            <verticals-tabs :currentSelection="currentSelection" slot="extraHeader"></verticals-tabs>
        </main-header>
</template>
<script src="./header.js"></script>


