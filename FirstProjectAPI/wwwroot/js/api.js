/* =========================================================
   FirstProjectAPI — helpers d'appel API partagés
   Servi en statique par la même app ASP.NET Core que l'API,
   donc les chemins relatifs ("/api/...") suffisent (même origine).
   ========================================================= */

const API_BASE = ""; // même origine : pas besoin d'URL absolue

const AUTH_STORAGE_KEY = "fpapi_auth";

/**
 * Sauvegarde la réponse d'authentification (token + infos user) en localStorage.
 * @param {object} authResponse - correspond à AuthResponseDto côté API
 */
function saveAuth(authResponse) {
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(authResponse));
}

/** Récupère les infos d'auth stockées, ou null si absentes. */
function getAuth() {
    const raw = localStorage.getItem(AUTH_STORAGE_KEY);
    if (!raw) return null;
    try {
        return JSON.parse(raw);
    } catch {
        return null;
    }
}

/** Supprime les infos d'auth stockées (déconnexion). */
function clearAuth() {
    localStorage.removeItem(AUTH_STORAGE_KEY);
}

/** Redirige vers la page de login si aucun token n'est présent. À appeler en haut des pages protégées. */
function requireAuth() {
    const auth = getAuth();
    if (!auth || !auth.token) {
        window.location.href = "/index.html";
        return null;
    }
    return auth;
}

/**
 * Appel générique vers l'API. Ajoute automatiquement le header Authorization
 * si un token est stocké. Lève une erreur avec le message serveur en cas d'échec.
 */
async function apiFetch(path, options = {}) {
    const auth = getAuth();
    const headers = {
        "Content-Type": "application/json",
        ...(options.headers || {}),
    };

    if (auth && auth.token) {
        headers["Authorization"] = `Bearer ${auth.token}`;
    }

    const response = await fetch(API_BASE + path, {
        ...options,
        headers,
    });

    let body = null;
    const contentType = response.headers.get("content-type") || "";
    if (contentType.includes("application/json")) {
        body = await response.json().catch(() => null);
    } else {
        body = await response.text().catch(() => null);
    }

    if (!response.ok) {
        const message =
            (body && typeof body === "object" && (body.title || body.message)) ||
            (typeof body === "string" && body) ||
            `Erreur ${response.status}`;
        const err = new Error(message);
        err.status = response.status;
        err.body = body;
        throw err;
    }

    return body;
}

/* ---------- Authentification ---------- */

/** POST /api/auth/register — Role omis => Apprenant par défaut côté serveur. */
function registerRequest({ name, email, password }) {
    return apiFetch("/api/auth/register", {
        method: "POST",
        body: JSON.stringify({ name, email, password }),
    });
}

/** POST /api/auth/login */
function loginRequest({ email, password }) {
    return apiFetch("/api/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });
}

/** GET /api/auth/me */
function meRequest() {
    return apiFetch("/api/auth/me", { method: "GET" });
}

/** POST /api/auth/register-staff — réservé Admin. Body: { name, email, password, role } (role: 1=Formateur, 2=Admin) */
function registerStaffRequest({ name, email, password, role }) {
    return apiFetch("/api/auth/register-staff", {
        method: "POST",
        body: JSON.stringify({ name, email, password, role }),
    });
}
/* ---------- Rôles ---------- */

/** Normalise un rôle (int ou string, venant du login ou de /me) en libellé FR. */
function roleLabel(role) {
    const map = {
        0: "Apprenant",
        1: "Formateur",
        2: "Admin",
        Apprenant: "Apprenant",
        Formateur: "Formateur",
        Admin: "Admin",
    };
    return map[role] ?? String(role);
}

/** true si le rôle donné (int ou string) fait partie de la liste de noms autorisés. */
function roleIn(role, allowedNames) {
    return allowedNames.includes(roleLabel(role));
}

/* ---------- Dashboard ---------- */

/** GET /api/dashboard — réservé Admin. Renvoie les totaux globaux de la plateforme. */
function dashboardStatsRequest() {
    return apiFetch("/api/dashboard", { method: "GET" });
}

/* ---------- Catégories & Formateurs ---------- */

/** GET /api/categorie */
function categoriesRequest() {
    return apiFetch("/api/categorie", { method: "GET" });
}

/** POST /api/categorie — réservé Admin. */
function createCategorieRequest({ name }) {
    return apiFetch("/api/categorie", {
        method: "POST",
        body: JSON.stringify({ name }),
    });
}

/** PUT /api/categorie/{id} — réservé Admin. */
function updateCategorieRequest(id, { name }) {
    return apiFetch(`/api/categorie/${id}`, {
        method: "PUT",
        body: JSON.stringify({ name }),
    });
}

/** DELETE /api/categorie/{id} — réservé Admin. */
function deleteCategorieRequest(id) {
    return apiFetch(`/api/categorie/${id}`, { method: "DELETE" });
}

/** GET /api/formateur */
function formateursRequest() {
    return apiFetch("/api/formateur", { method: "GET" });
}

/** POST /api/formateur — réservé Admin. Body: CreateFormateurDto. */
function createFormateurRequest(payload) {
    return apiFetch("/api/formateur", {
        method: "POST",
        body: JSON.stringify(payload),
    });
}

/** PUT /api/formateur/{id} — réservé Admin. Body: UpdateFormateurDto. */
function updateFormateurRequest(id, payload) {
    return apiFetch(`/api/formateur/${id}`, {
        method: "PUT",
        body: JSON.stringify(payload),
    });
}

/** DELETE /api/formateur/{id} — réservé Admin. */
function deleteFormateurRequest(id) {
    return apiFetch(`/api/formateur/${id}`, { method: "DELETE" });
}

/* ---------- Formations ---------- */

/** GET /api/formation */
function formationsRequest() {
    return apiFetch("/api/formation", { method: "GET" });
}

/** POST /api/formation/{idCategorie} — idCategorie vient de la route, pas du body. */
function createFormationRequest(idCategorie, { name, formateurId }) {
    return apiFetch(`/api/formation/${idCategorie}`, {
        method: "POST",
        body: JSON.stringify({ name, formateurId: formateurId || null }),
    });
}

/** PUT /api/formation/{id} — le backend ne met à jour que Name et IdCategorie (pas FormateurId). */
function updateFormationRequest(id, { name, idCategorie }) {
    return apiFetch(`/api/formation/${id}`, {
        method: "PUT",
        body: JSON.stringify({ name, idCategorie }),
    });
}

/** DELETE /api/formation/{id} — réservé Admin. */
function deleteFormationRequest(id) {
    return apiFetch(`/api/formation/${id}`, { method: "DELETE" });
}

/* ---------- Modules ---------- */

/** GET /api/module/formation/{idFormation} */
function modulesByFormationRequest(idFormation) {
    return apiFetch(`/api/module/formation/${idFormation}`, { method: "GET" });
}

/** POST /api/module/formation/{idFormation} — body: { titre, ordre } */
function createModuleRequest(idFormation, { titre, ordre }) {
    return apiFetch(`/api/module/formation/${idFormation}`, {
        method: "POST",
        body: JSON.stringify({ titre, ordre }),
    });
}

/** PUT /api/module/{id} */
function updateModuleRequest(id, { titre, ordre }) {
    return apiFetch(`/api/module/${id}`, {
        method: "PUT",
        body: JSON.stringify({ titre, ordre }),
    });
}

/** DELETE /api/module/{id} */
function deleteModuleRequest(id) {
    return apiFetch(`/api/module/${id}`, { method: "DELETE" });
}

/* ---------- Modalités ---------- */

/** GET /api/modalite/module/{idModule} */
function modalitesByModuleRequest(idModule) {
    return apiFetch(`/api/modalite/module/${idModule}`, { method: "GET" });
}

/** POST /api/modalite/module/{idModule} — body: { titre, type, ordre, contenu, dureeMinutes } */
function createModaliteRequest(idModule, payload) {
    return apiFetch(`/api/modalite/module/${idModule}`, {
        method: "POST",
        body: JSON.stringify(payload),
    });
}

/** PUT /api/modalite/{id} */
function updateModaliteRequest(id, payload) {
    return apiFetch(`/api/modalite/${id}`, {
        method: "PUT",
        body: JSON.stringify(payload),
    });
}

/** DELETE /api/modalite/{id} */
function deleteModaliteRequest(id) {
    return apiFetch(`/api/modalite/${id}`, { method: "DELETE" });
}
/* ---------- Questions ---------- */

/** GET /api/question/modalite/{idModalite} */
function questionsByModaliteRequest(idModalite) {
    return apiFetch(`/api/question/modalite/${idModalite}`, { method: "GET" });
}

/** POST /api/question/modalite/{idModalite} — réservé Admin/Formateur. Body: Question (sans Id/IdModalite). */
function createQuestionRequest(idModalite, payload) {
    return apiFetch(`/api/question/modalite/${idModalite}`, {
        method: "POST",
        body: JSON.stringify(payload),
    });
}

/** PUT /api/question/{id} — réservé Admin/Formateur. */
function updateQuestionRequest(id, payload) {
    return apiFetch(`/api/question/${id}`, {
        method: "PUT",
        body: JSON.stringify(payload),
    });
}

/** DELETE /api/question/{id} — réservé Admin/Formateur. */
function deleteQuestionRequest(id) {
    return apiFetch(`/api/question/${id}`, { method: "DELETE" });
}

/* ---------- Réponses apprenant ---------- */

/** POST /api/ReponseApprenant/repondre — réservé Apprenant. Body: { idQuestion, reponseDonnee }. */
function repondreRequest(idQuestion, reponseDonnee) {
    return apiFetch("/api/ReponseApprenant/repondre", {
        method: "POST",
        body: JSON.stringify({ idQuestion, reponseDonnee }),
    });
}

/** GET /api/ReponseApprenant/resultat/{idModalite} — Apprenant (le sien), Formateur/Admin. */
function resultatRequest(idModalite) {
    return apiFetch(`/api/ReponseApprenant/resultat/${idModalite}`, { method: "GET" });
}

/* ---------- Avatar IA ---------- */

/** POST /api/avatar/session/start — réservé Apprenant. */
function startAvatarSessionRequest() {
    return apiFetch("/api/avatar/session/start", { method: "POST" });
}

/** POST /api/avatar/speak — réservé Apprenant. Body: { question } */
function speakAvatarRequest(question) {
    return apiFetch("/api/avatar/speak", {
        method: "POST",
        body: JSON.stringify({ question }),
    });
}

/** POST /api/avatar/stop — réservé Apprenant. */
function stopAvatarSessionRequest() {
    return apiFetch("/api/avatar/stop", { method: "POST" });
}
/* ---------- Questions & Réponses (exercices/examens) ---------- */

/** GET /api/question/modalite/{idModalite} */
function questionsByModaliteRequest(idModalite) {
    return apiFetch(`/api/question/modalite/${idModalite}`, { method: "GET" });
}

/** POST /api/ReponseApprenant/repondre — réservé Apprenant. Body: { idQuestion, reponseDonnee } */
function repondreRequest({ idQuestion, reponseDonnee }) {
    return apiFetch("/api/ReponseApprenant/repondre", {
        method: "POST",
        body: JSON.stringify({ idQuestion, reponseDonnee }),
    });
}

/** GET /api/ReponseApprenant/resultat/{idModalite} */
function resultatRequest(idModalite) {
    return apiFetch(`/api/ReponseApprenant/resultat/${idModalite}`, { method: "GET" });
}
/* ---------- Sessions ---------- */

/** GET /api/session — visible par tout utilisateur connecté. */
function sessionsRequest() {
    return apiFetch("/api/session", { method: "GET" });
}

/** POST /api/session — réservé Admin. Body: { dateDebut, duree, idFormation } */
function createSessionRequest(payload) {
    return apiFetch("/api/session", {
        method: "POST",
        body: JSON.stringify(payload),
    });
}

/** PUT /api/session/{id} — réservé Admin. */
function updateSessionRequest(id, payload) {
    return apiFetch(`/api/session/${id}`, {
        method: "PUT",
        body: JSON.stringify(payload),
    });
}

/** DELETE /api/session/{id} — réservé Admin. */
function deleteSessionRequest(id) {
    return apiFetch(`/api/session/${id}`, { method: "DELETE" });
}

/** POST /api/session/{id}/inscription — réservé Apprenant. L'apprenant connecté s'inscrit. */
function inscrireSessionRequest(id) {
    return apiFetch(`/api/session/${id}/inscription`, { method: "POST" });
}

/** GET /api/session/{id}/participants — réservé Admin/Formateur. */
function participantsRequest(id) {
    return apiFetch(`/api/session/${id}/participants`, { method: "GET" });
}

/** GET /api/session/mes-sessions — réservé Apprenant. */
function mesSessionsRequest() {
    return apiFetch("/api/session/mes-sessions", { method: "GET" });
}
