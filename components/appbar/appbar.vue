<template>
  <div id="appbar">
    <v-app-bar color="#4d0026" flat>
      <img src="~/assets/logo.png" />
      <div class="txtbar">LCIT INVOICE EDI MANAGEMENT SYSTEM</div>
    </v-app-bar>
  </div>
</template>

<script>
export default {
  name: 'appbar',
  // dialog: false,

  data() {
    return {}
  },

  methods: {},
}
</script>

<style>
.txtbar {
  font-weight: bold;
  margin-left: 10px;
  color: white;
}
</style>
