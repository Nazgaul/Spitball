<template>
        <main-header ref="mainHeader" :userText="userText" 
                     :submitRoute="submitRoute" :currentSelection="currentSelection"
                     :toolbarHeight="$vuetify.breakpoint.smAndUp ? 104 : 144">
            <verticals-tabs :currentSelection="currentSelection" slot="extraHeader"></verticals-tabs>
        </main-header>
</template>
<script src="./header.js"></script>


